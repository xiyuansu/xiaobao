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
    /// 订单费用获取(包括费用明细)
    /// </summary>
    public class GetFee : IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();
            string plate_no = params_ht["plate_no"].ToString();
            string order_no = params_ht["order_no"].ToString();

            GetFeeData data = new GetFeeData();

            BaoJiaManager mgr = new BaoJiaManager();
            Hashtable ht = mgr.GetOrderInfo(order_no);
            if (ht == null)
            {
                resp.msg = "未找到相关订单";
                resp.code = "-1";
            }
            else
            {
                decimal kmprice = 0.80m, minuteprice = 0.40m, mileage = 0, TotalMoney = 0.00m, SettlementMoney = 0.00m, outServiceAreaFee = 0.00m;
                int minutes = 0;
                string returnLocType = "",lng="0",lat="0";
                ServiceAreaManager areaManager = new ServiceAreaManager();
                SysSettingManager settingManager = new SysSettingManager();
                VehiclePriceManager priceManager = new VehiclePriceManager();
                Hashtable billing_ht = priceManager.GetOrderSettlement(SiteHelper.GetHashTableValueByKey(ht, "id"));
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "MinutePrice"), out minuteprice);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "KmPrice"), out kmprice);
                int.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "Minutes"), out minutes);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "TotalMoney"), out TotalMoney);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "SettlementMoney"), out SettlementMoney);
                returnLocType=SiteHelper.GetHashTableValueByKey(ht, "ReturnLocType");
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "OutServiceAreaFee"), out outServiceAreaFee);

                string orderState = SiteHelper.GetHashTableValueByKey(ht, "OrderState");
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByIDORNumber(SiteHelper.GetHashTableValueByKey(ht, "VehicleID"));
                lat = vehicle_ht["LATITUDE"].ToString();
                lng = vehicle_ht["LONGITUDE"].ToString();
                if (orderState == "1")
                {
                    SettlementMoney = TotalMoney;
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
                SettlementMoney = Math.Round(SettlementMoney, 2);
                data.order_no = order_no;
                data.all_mile = double.Parse(SiteHelper.GetHashTableValueByKey(ht, "Mileage"));
                data.location_address_type = "3";
                if (returnLocType == "01")
                    data.location_address_type = "3";
                if (returnLocType == "02" || returnLocType == "03")
                    data.location_address_type = "1";
                if (returnLocType=="04")
                    data.location_address_type = "2";
                
                data.location_address_msg = "";
                data.order_status = 10000;
                if (orderState == "1")
                    data.order_status = 10000;
                else if (orderState == "0")
                    data.order_status = 10301;
                else if (orderState == "2" || orderState == "5" || orderState == "3")
                    data.order_status = 80200;
                data.total_fee = (double)(SettlementMoney + outServiceAreaFee);
                data.timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
                data.lng = double.Parse(lng);
                data.lat = double.Parse(lat);

                data.details = new List<GetFeeData_Fee>();
                GetFeeData_Fee item1 = new GetFeeData_Fee();
                item1.name = "用车费用";
                item1.amount = (double)SettlementMoney;
                data.details.Add(item1);
                if (returnLocType == "04")
                {
                    GetFeeData_Fee item2 = new GetFeeData_Fee();
                    item2.name = "行驶区域外还车调度费";
                    item2.amount = (double)(outServiceAreaFee);
                    data.details.Add(item2);
                }
                if (returnLocType == "02")
                {
                    GetFeeData_Fee item3 = new GetFeeData_Fee();
                    item3.name = "停车点外还车调度费";
                    item3.amount = (double)(outServiceAreaFee);
                    data.details.Add(item3);
                }

                data.isLock = "1";
                int cnt=-1;
                DataTable parking_dt= areaManager.GetNearParking(9999,lng.ToString(), lat.ToString(),1,9999,ref cnt);
                if(parking_dt!=null && parking_dt.Rows.Count>0)
                {
                    List<GetFeeData_StationItem> station_list = new List<GetFeeData_StationItem>();
                    foreach(DataRow dr in parking_dt.Rows)
                    {
                        GetFeeData_StationItem station = new GetFeeData_StationItem();
                        station.parkType = "2";
                        station.coordinatePoints = dr["Coordinates"].ToString().Replace(";","|");
                        station.coordinate_center = dr["Longitude"].ToString() + "," + dr["Latitude"].ToString();
                        station_list.Add(station);
                    }
                    data.stationList = station_list;
                }

                resp.data = data;
                resp.code = "0";
            }

            return resp;
        }
    }

    public class GetFeeData
    {
        public string order_no;

        public double all_mile;

        public double lng;

        public double lat;

        public string location_address_type;

        public string location_address_msg;

        public int order_status;

        public double total_fee;

        public long timestamp;

        public List<GetFeeData_Fee> details;

        public string isLock;

        public List<GetFeeData_StationItem> stationList;
    }

    public class GetFeeData_Fee
    {
        public string name;

        public double amount;
    }

    public class GetFeeData_StationItem
    {
        public string parkType;

        public string radius;

        public string coordinatePoints;

        public string coordinate_center;
    }

}