using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using YR.Common.DotNetCache;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 用户还车
    /// </summary>
    public class ReturnVehicle : IApiAction2
    {
        private static Log Logger = LogFactory.GetLogger(typeof(ReturnVehicle));

        private string uid = string.Empty;

        private string userLng = string.Empty;

        private string userLat = string.Empty;
        private string cacheKey = "ReturnVehicle_";

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
                if (res["Longitude"] != null && res["Longitude"].ToString().Trim().Length > 0)
                    userLng = res["Longitude"].ToString().Trim();
                if (res["Latitude"] != null && res["Latitude"].ToString().Trim().Length > 0)
                    userLat = res["Latitude"].ToString().Trim();
                cacheKey += uid;
                ICache cache = null;
                cache = CacheFactory.GetCache();
                if (!string.IsNullOrEmpty(cache.Get<string>(cacheKey)))
                {
                    cache.Dispose();
                    Logger.Error("还车10秒内，" + uid);
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "还车处理中，请勿重复点击");
                }
                else
                {
                    DateTime dt = DateTime.Now.AddSeconds(10);
                    cache.Set(cacheKey, uid, dt - DateTime.Now);
                    cache.Dispose();
                }
                if (cache != null)
                {
                    cache.Dispose();
                }
                UserInfoManager uim = new UserInfoManager();
                OrdersManager om = new OrdersManager();
                VehicleManager vm = new VehicleManager();
                ServiceAreaManager areaManager = new ServiceAreaManager();
                SysSettingManager settingManager = new SysSettingManager();
                Hashtable order = om.GetUserCurrentOrderVehicle(uid);
                //判断是否有车
                if (order == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您没有车辆无法还车");
                }
                else
                {
                    //车辆坐标
                    string longitude = SiteHelper.GetHashTableValueByKey(order, "Longitude");
                    string latitude = SiteHelper.GetHashTableValueByKey(order, "Latitude");

                    Hashtable userht = uim.GetUserInfoByUserID(uid);
                    decimal balance = 0.00m, raiseBalance = 0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userht, "Balance"), out balance);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userht, "RaiseBalance"), out raiseBalance);
                    //处理订单
                    if (order == null)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "还车操作失败,请联系管理员");
                    }
                    else
                    {
                        int renttime = 0;
                        int.TryParse(SiteHelper.GetHashTableValueByKey(order, "RentDuration"), out renttime);
                        decimal hours = 0.00m;
                        decimal kmprice = 0.80m;
                        decimal minuteprice = 0.40m;
                        decimal mileage = 0;
                        int minutes = 0;
                        decimal userprivilegeratio = 1;
                        decimal raisecommisssionratio = 0.10m;
                        decimal TotalMoney = 0.00m;
                        decimal SettlementMoney = 0.00m;
                        decimal occupyMoney = 0.0m;
                        decimal payMoney = 0.00m;
                        decimal RaiseCommissionMoney = 0.00m;
                        decimal outServiceAreaFee = 0.00m;
                        DateTime createtime = DateTime.Now;
                        DateTime startTime = DateTime.Now;
                        String returnParkingID = null, returnLocType = null;
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "KMPrice"), out kmprice);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "MinutePrice"), out minuteprice);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "Mileage"), out mileage);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "UserPrivilegeRatio"), out userprivilegeratio);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "RaiseCommissionRatio"), out raisecommisssionratio);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "TotalMoney"), out TotalMoney);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "SettlementMoney"), out SettlementMoney);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "OccupyMoney"), out occupyMoney);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "RaiseCommissionMoney"), out RaiseCommissionMoney);
                        DateTime.TryParse(SiteHelper.GetHashTableValueByKey(order, "CreateTime"), out createtime);
                        DateTime.TryParse(SiteHelper.GetHashTableValueByKey(order, "StartTime"), out startTime);
                        int startminutes = 0;
                        int.TryParse(SiteHelper.GetHashTableValueByKey(order, "StartMinutes"), out startminutes);
                        string strStartTime = SiteHelper.GetHashTableValueByKey(order, "StartTime");

                        LatLng vehicle_pt = new LatLng(double.Parse(latitude), double.Parse(longitude));
                        //车辆GPS坐标转高德坐标
                        vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
                        vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                        Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(SiteHelper.GetHashTableValueByKey(order, "VID"));
                        List<LatLng> area_pts = new List<LatLng>();
                        bool isInPoly = true;
                        string outServiceAreaReturn = settingManager.GetValueByKey("OutServiceAreaReturn");
                        if (area_ht != null && area_ht.Keys.Count > 0)
                        {
                            string coordinates = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates");
                            foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                area_pts.Add(pt);
                            }
                            isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                            if (!isInPoly && "0".Equals(outServiceAreaReturn))
                            {
                                Logger.Info("用户:" + uid + ",订单ID:" + SiteHelper.GetHashTableValueByKey(order, "OrderID") + ",在运营区域外被禁止还车");
                                return SiteHelper.GetJsonFromHashTable(null, "faild", "运营区域外禁止还车,请骑回运营区域再还车");
                            }
                        }

                        string gpsnum = SiteHelper.GetHashTableValueByKey(order, "VehicleGPSNum");
                        //锁车
                        bool isSuccess = vm.CloseVehicle(SiteHelper.GetHashTableValueByKey(order, "VID"));
                        if (!isSuccess)
                        {
                            Logger.Error("用户:" + uid + ",订单ID:" + SiteHelper.GetHashTableValueByKey(order, "OrderID") + ",还车锁车失败:" + gpsnum);
                        }
                        VehiclePriceManager priceManager = new VehiclePriceManager();
                        Hashtable billing_ht = priceManager.GetOrderSettlement(SiteHelper.GetHashTableValueByKey(order, "OrderID"));
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "MinutePrice"), out minuteprice);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "KmPrice"), out kmprice);
                        int.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "Minutes"), out minutes);
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "TotalMoney"), out TotalMoney);

                        SettlementMoney = TotalMoney;

                        #region 判断还车点是否在运营区域内，运营区域外加收费用
                        if (TotalMoney > 0 && area_ht != null && area_ht.Keys.Count > 0)
                        {
                            returnLocType = "03";
                            //bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                            string vehicleInArea = isInPoly ? "在运营区域内还车" : "在运营区域外还车";
                            Logger.Info("用户:" + uid + ",订单ID:" + SiteHelper.GetHashTableValueByKey(order, "OrderID") + "," + vehicleInArea + "车辆GPS坐标:" + longitude + "," + latitude + ",高德坐标:" + vehicle_pt.longitude + "," + vehicle_pt.latitude);
                            bool isInPoly2 = false;//用户坐标是否在区域内
                            if (!string.IsNullOrEmpty(userLng) && !string.IsNullOrEmpty(userLat))
                            {
                                LatLng user_pt = new LatLng(double.Parse(userLat), double.Parse(userLng));
                                isInPoly2 = SiteHelper.IsPtInPoly(user_pt, area_pts);
                            }
                            //string userInArea = isInPoly2 ? "用户在运营区域内" : "用户在运营区域外";

                            //车辆在运营区域外，不判断用户位置  if (!isInPoly && !isInPoly2)
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
                                Logger.Info("订单ID: " + SiteHelper.GetHashTableValueByKey(order, "OrderID") + ",加收调度费值:" + serviceFee + ",加收调度费金额:" + outServiceAreaFee);
                            }
                        }
                        #endregion

                        #region 判断还车点是否在停车点内，否则加收费用
                        string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
                        if (TotalMoney > 0 && outServiceAreaFee == 0 && returnVehicleMode == "1")
                        {
                            returnLocType = "01";
                            DataTable parking_dt = areaManager.GetNearestParkingList(vehicle_pt.longitude.ToString(), vehicle_pt.latitude.ToString(), SiteHelper.GetHashTableValueByKey(order, "VID"));
                            if (parking_dt != null)
                            {
                                //bool isInPoly = false;
                                bool isInPoly2 = false;//用户坐标是否在区域内
                                foreach (DataRow dr in parking_dt.Rows)
                                {
                                    List<LatLng> parking_pts = new List<LatLng>();
                                    returnParkingID = dr["ID"].ToString();
                                    string coordinates = dr["Coordinates"].ToString();
                                    foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                        parking_pts.Add(pt);
                                    }
                                    isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, parking_pts);
                                    if (!string.IsNullOrEmpty(userLng) && !string.IsNullOrEmpty(userLat))
                                    {
                                        LatLng user_pt = new LatLng(double.Parse(userLat), double.Parse(userLng));
                                        isInPoly2 = SiteHelper.IsPtInPoly(user_pt, parking_pts);
                                    }

                                    string vehicleInArea = isInPoly ? "在停车点内还车" : "在停车点外还车";
                                    string userInArea = isInPoly2 ? "用户在停车点内" : "用户在停车点外";
                                    Logger.Info("用户:" + uid + ",订单ID:" + SiteHelper.GetHashTableValueByKey(order, "OrderID") + "," + vehicleInArea + ",车辆GPS坐标:" + longitude + "," + latitude + ",高德坐标:" + vehicle_pt.longitude + "," + vehicle_pt.latitude + ",用户高德坐标:" + userLng + "," + userLat + "," + userInArea);
                                    if (isInPoly || isInPoly2)
                                    {
                                        break;
                                    }
                                }
                                if (!isInPoly && !isInPoly2)
                                {
                                    returnLocType = "02";
                                    returnParkingID = null;
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
                                    Logger.Info("用户:" + uid + ",订单ID:" + SiteHelper.GetHashTableValueByKey(order, "OrderID") + "加收调度费值:" + serviceFee + ",加收调度费金额:" + outServiceAreaFee);
                                }
                            }
                        }
                        #endregion

                        payMoney = TotalMoney;

                        Hashtable userParm = new Hashtable();
                        userParm["VID"] = SiteHelper.GetHashTableValueByKey(order, "VID");
                        userParm["OrderNum"] = SiteHelper.GetHashTableValueByKey(order, "OrderNum");
                        userParm["TotalMoney"] = TotalMoney;
                        userParm["SettlementMoney"] = SettlementMoney;
                        userParm["PayMoney"] = payMoney;
                        userParm["PayTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                        userParm["EndTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                        userParm["FinishedTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                        userParm["OID"] = SiteHelper.GetHashTableValueByKey(order, "orderid");
                        userParm["ChangesAmount"] = -Math.Abs(TotalMoney);
                        userParm["UserFinancialID"] = CommonHelper.GetGuid;
                        userParm["UserID"] = uid;
                        userParm["ChangesTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                        userParm["ChangesType"] = UserFinancialChangesType.Consumption.GetHashCode().ToString();
                        userParm["OrderID"] = SiteHelper.GetHashTableValueByKey(order, "orderid");
                        userParm["RentDuration"] = hours;
                        userParm["Remark"] = "用户还车";
                        userParm["Minutes"] = minutes;
                        userParm["DiffMoney"] = TotalMoney;
                        userParm["Operator"] = "";
                        userParm["OperatorType"] = UserFinancialOperatorType.Admin.GetHashCode().ToString();
                        userParm["State"] = UserFinancialState.Effect.GetHashCode().ToString();
                        userParm["MessageID"] = CommonHelper.GetGuid;
                        userParm["OutServiceAreaFee"] = (int)outServiceAreaFee;
                        userParm["ReturnParkingID"] = returnParkingID;
                        userParm["ReturnLocType"] = returnLocType;

                        bool isSettlementSuccess = uim.ReturnVehicle(userParm);
                        if (isSettlementSuccess)
                        {
                            om.UpdateOrderGPS(SiteHelper.GetHashTableValueByKey(order, "orderid"), false);

                            Hashtable ht = om.GetOrderByNum(SiteHelper.GetHashTableValueByKey(order, "OrderNum"));

                            return SiteHelper.GetJsonFromHashTable(ht, "success", "还车成功", "OrderInfo");
                        }
                        else
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "还车失败,请联系管理员");
                        }
                    }
                }
            }
        }

    }
}