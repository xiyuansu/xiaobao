using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;

namespace YR.Web.api.baojia.service
{
    /// <summary>
    /// 批量获取订单状态信息
    /// </summary>
    public class GetBatchOrderState : IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();
            string batch_order_no = params_ht["batch_order_no"].ToString();

            List<GetBatchOrderState_Item> data = new List<GetBatchOrderState_Item>();
            if(!string.IsNullOrEmpty(batch_order_no))
            {
                foreach(string order_no in batch_order_no.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries))
                {
                    GetBatchOrderState_Item item=GetOrderState(order_no);
                    if (item != null)
                        data.Add(item);
                }
                resp.data = data;
                resp.code = "0";
            }
            else
            {
                resp.msg = "订单号未填写";
                resp.code = "-1";
            }
            

            return resp;
        }

        private GetBatchOrderState_Item GetOrderState(string order_no)
        {
            BaoJiaManager mgr = new BaoJiaManager();
            Hashtable ht = mgr.GetOrderInfo(order_no);
            if (ht == null)
            {
                return null;
            }
            else
            {
                GetBatchOrderState_Item data = new GetBatchOrderState_Item();

                decimal kmprice = 0.80m, minuteprice = 0.40m, mileage = 0, TotalMoney = 0.00m, SettlementMoney = 0.00m, outServiceAreaFee = 0.00m;
                int minutes = 0;
                string returnLocType = "";
                ServiceAreaManager areaManager = new ServiceAreaManager();
                SysSettingManager settingManager = new SysSettingManager();
                VehiclePriceManager priceManager = new VehiclePriceManager();
                Hashtable billing_ht = priceManager.GetOrderSettlement(SiteHelper.GetHashTableValueByKey(ht, "id"));
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "MinutePrice"), out minuteprice);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "KmPrice"), out kmprice);
                int.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "Minutes"), out minutes);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "TotalMoney"), out TotalMoney);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "SettlementMoney"), out SettlementMoney);
                returnLocType = SiteHelper.GetHashTableValueByKey(ht, "ReturnLocType");
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "OutServiceAreaFee"), out outServiceAreaFee);

                string orderState = SiteHelper.GetHashTableValueByKey(ht, "OrderState");
                if (orderState == "1")
                {
                    SettlementMoney = TotalMoney;
                    VehicleManager vm = new VehicleManager();
                    Hashtable vehicle_ht = vm.GetVehicleInfoByIDORNumber(SiteHelper.GetHashTableValueByKey(ht, "VehicleID"));
                    string lat = vehicle_ht["LATITUDE"].ToString();
                    string lng = vehicle_ht["LONGITUDE"].ToString();

                    #region 判断还车点是否在运营区域内，运营区域外加收费用
                    Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(SiteHelper.GetHashTableValueByKey(ht, "VehicleID"));
                    if (area_ht != null && area_ht.Keys.Count > 0)
                    {
                        returnLocType = "03";
                        List<LatLng> area_pts = new List<LatLng>();
                        string coordinates = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates");
                        foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                            area_pts.Add(pt);
                        }
                        LatLng vehicle_pt = new LatLng(double.Parse(lat), double.Parse(lng));
                        //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                        vehicle_pt = SiteHelper.GPSToGCJ02(lng, lat);
                        vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                        bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                        if (!isInPoly)
                        {
                            returnLocType = "04";
                            string serviceFee = settingManager.GetValueByKey("OutServiceAreaFee");
                            if (serviceFee.StartsWith("+"))
                            {
                                serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                decimal.TryParse(serviceFee, out outServiceAreaFee);
                                TotalMoney = TotalMoney + outServiceAreaFee;
                            }
                            else
                            {
                                serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                decimal.TryParse(serviceFee, out outServiceAreaFee);
                                TotalMoney = TotalMoney * outServiceAreaFee;
                            }
                            outServiceAreaFee = TotalMoney - SettlementMoney;
                        }
                    }
                    #endregion

                    #region 判断还车点是否在停车点内，否则加收费用
                    string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
                    if (outServiceAreaFee == 0 && returnVehicleMode == "1")
                    {
                        returnLocType = "01";
                        LatLng vehicle_pt = new LatLng(double.Parse(lat), double.Parse(lng));
                        //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                        vehicle_pt = SiteHelper.GPSToGCJ02(lng, lat);
                        vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                        Hashtable parking_ht = areaManager.GetNearestParking(vehicle_pt.longitude.ToString(), vehicle_pt.latitude.ToString(), SiteHelper.GetHashTableValueByKey(ht, "VehicleID"));

                        List<LatLng> parking_pts = new List<LatLng>();
                        if (parking_ht != null && parking_ht.Keys.Count > 0)
                        {
                            string coordinates = SiteHelper.GetHashTableValueByKey(parking_ht, "Coordinates");
                            foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                parking_pts.Add(pt);
                            }
                        }

                        bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, parking_pts);
                        if (!isInPoly)
                        {
                            returnLocType = "02";
                            string serviceFee = settingManager.GetValueByKey("OutParkingAreaFee");
                            if (serviceFee.StartsWith("+"))
                            {
                                serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                decimal.TryParse(serviceFee, out outServiceAreaFee);
                                TotalMoney = TotalMoney + outServiceAreaFee;
                            }
                            else
                            {
                                serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                decimal.TryParse(serviceFee, out outServiceAreaFee);
                                TotalMoney = TotalMoney * outServiceAreaFee;
                            }
                            outServiceAreaFee = TotalMoney - SettlementMoney;
                        }
                    }
                    #endregion
                }

                data.order_no = order_no;
                data.amount = (double)(SettlementMoney + outServiceAreaFee); ;
                
                data.order_status = 10000;
                if (orderState == "1")
                    data.order_status = 10000;
                else if (orderState == "0")
                    data.order_status = 10301;
                else if (orderState == "2" || orderState == "5" || orderState == "3")
                    data.order_status = 80200;
                
                data.timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;

                return data;
            }
        }
    }

    public class GetBatchOrderState_Item
    {
        public string order_no;

        public int order_status;

        public double amount;

        public long timestamp; 
    }

}