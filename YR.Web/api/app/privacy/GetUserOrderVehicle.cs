using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 得到用户预约的车辆
    /// </summary>
    public class GetUserOrderVehicle : IApiAction2
    {
        private string uid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                OrdersManager om = new OrdersManager();
                Hashtable ht = om.GetUserCurrentOrderVehicle(uid);
                if (ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前没有预约车辆");
                }
                else
                {
                    string lat = ht["LATITUDE"].ToString();
                    string lng = ht["LONGITUDE"].ToString();
                    if (!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat))
                    {
                        if (res["Client"] != null && "iOS" == res["Client"].ToString())
                        {
                            LatLng ll = new LatLng(double.Parse(lat), double.Parse(lng));
                            ll = SiteHelper.TransformFromWGSToGCJ(ll);
                            ht["LONGITUDE"] = ll.longitude;
                            ht["LATITUDE"] = ll.latitude;
                        }
                        else
                        {
                            /*LatLng latlng = SiteHelper.GetBaiduByGPS(lat, lng);
                            if (latlng != null)
                            {
                                ht["LATITUDE"] = latlng.latitude;
                                ht["LONGITUDE"] = latlng.longitude;
                                if (ht["ADDRESS"].ToString().Length <= 0)
                                {
                                    ht["ADDRESS"] = SiteHelper.GetLocationByGPS(ht["LATITUDE"].ToString(), ht["LONGITUDE"].ToString());
                                }
                            }*/
                            LatLng ll = new LatLng(double.Parse(lat), double.Parse(lng));
                            ll = SiteHelper.TransformFromWGSToGCJ(ll);
                            ht["LONGITUDE"] = ll.longitude;
                            ht["LATITUDE"] = ll.latitude;
                            ht["ADDRESS"] = SiteHelper.GetLocationByGPS(ht["LATITUDE"].ToString(), ht["LONGITUDE"].ToString(), "wgs84ll");
                        }
                    }

                    decimal kmprice = 0.80m,minuteprice = 0.40m,mileage = 0,TotalMoney = 0.00m,SettlementMoney = 0.00m,outServiceAreaFee = 0.00m;
                    int minutes = 0;
                    ServiceAreaManager areaManager = new ServiceAreaManager();
                    SysSettingManager settingManager = new SysSettingManager();
                    VehiclePriceManager priceManager = new VehiclePriceManager();
                    Hashtable billing_ht = priceManager.GetOrderSettlement(SiteHelper.GetHashTableValueByKey(ht, "orderid"));
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "MinutePrice"), out minuteprice);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "KmPrice"), out kmprice);
                    int.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "Minutes"), out minutes);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "TotalMoney"), out TotalMoney);

                    SettlementMoney = TotalMoney;

                    #region 判断还车点是否在运营区域内，运营区域外加收费用
                    Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(SiteHelper.GetHashTableValueByKey(ht, "VID"));
                    if (area_ht != null && area_ht.Keys.Count > 0)
                    {
                        List<LatLng> area_pts = new List<LatLng>();
                        string coordinates = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates");
                        foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            LatLng pt = new LatLng(double.Parse(pt_arr[1]),double.Parse(pt_arr[0]));
                            area_pts.Add(pt);
                        }
                        LatLng vehicle_pt = new LatLng(double.Parse(lat), double.Parse(lng));
                        //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                        vehicle_pt = SiteHelper.GPSToGCJ02(lng, lat);
                        vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                        bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                        if (!isInPoly)
                        {
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
                    if (TotalMoney > 0 && outServiceAreaFee == 0 && returnVehicleMode == "1")
                    {
                        LatLng vehicle_pt = new LatLng(double.Parse(lat), double.Parse(lng));
                        //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                        vehicle_pt = SiteHelper.GPSToGCJ02(lng, lat);
                        vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                        /*Hashtable parking_ht = areaManager.GetNearestParking(vehicle_pt.longitude.ToString(), vehicle_pt.latitude.ToString(), SiteHelper.GetHashTableValueByKey(ht, "VID"));
                        List<LatLng> parking_pts = new List<LatLng>();
                        if (parking_ht != null && parking_ht.Keys.Count > 0)
                        {
                            returnParkingID = SiteHelper.GetHashTableValueByKey(parking_ht, "ID");
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
                        }*/
                        DataTable parking_dt = areaManager.GetNearestParkingList(vehicle_pt.longitude.ToString(), vehicle_pt.latitude.ToString(), SiteHelper.GetHashTableValueByKey(ht, "VID"));
                        if (parking_dt != null)
                        {
                            bool isInPoly = false;
                            foreach (DataRow dr in parking_dt.Rows)
                            {
                                List<LatLng> parking_pts = new List<LatLng>();
                                string coordinates = dr["Coordinates"].ToString();
                                foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                    parking_pts.Add(pt);
                                }
                                isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, parking_pts);
                                if (isInPoly)
                                {
                                    break;
                                }
                            }
                            if (!isInPoly)
                            {
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
                    }
                    #endregion

                    ht["MINUTES"] = minutes;
                    ht["OUTSERVICEAREAFEE"] =(int)outServiceAreaFee;
                    ht["TOTALMONEY"]=TotalMoney;

                    return SiteHelper.GetJsonFromHashTable(ht, "success", "获取数据成功", "OrderVehicle");
                }
            }
        }
    }
}