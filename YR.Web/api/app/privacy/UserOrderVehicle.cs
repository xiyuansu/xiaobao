using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
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
    /// 用户预约车辆
    /// </summary>
    public class UserOrderVehicle : IApiAction2
    {
        private string uid = string.Empty;

        private string vehicleid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null ||
                res["VehicleID"] == null ||
                res["UID"].ToString().Trim().Length <= 0 ||
                res["VehicleID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                vehicleid = res["VehicleID"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                OrdersManager om = new OrdersManager();
                UserInfoManager uim = new UserInfoManager();
                SysSettingManager settingManager = new SysSettingManager();
                Hashtable vehicle = vm.GetVehicleInfoByID(vehicleid);
                if (vehicle == null || vehicle.Keys.Count == 0)
                {
                    //根据车牌号码查询车辆信息
                    vehicle = vm.GetVehicleInfoByLicenseNumber(vehicleid);
                }
                //判断车辆是否存在
                if (vehicle == null || vehicle.Keys.Count == 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆预约失败,未找到车辆相关信息");
                }

                vehicleid = SiteHelper.GetHashTableValueByKey(vehicle, "ID");
                //判断预约车辆是否离线
                DateTime lastUpdateTime;
                if (string.IsNullOrEmpty(SiteHelper.GetHashTableValueByKey(vehicle, "LastUpdateTime")))
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "当前车辆已离线无法约车");
                }
                else
                {
                    lastUpdateTime = DateTime.Parse(SiteHelper.GetHashTableValueByKey(vehicle, "LastUpdateTime"));
                    if (SiteHelper.ExecDateDiff(lastUpdateTime, DateTime.Now) > 10)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "当前车辆已离线无法约车");
                    }
                }

                //判断预约车辆电量值
                double electricity=0.00;
                double.TryParse(SiteHelper.GetHashTableValueByKey(vehicle, "Electricity"), out electricity);
                if(electricity<30)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "当前车辆电量过低无法约车");
                }

                #region 判断车辆是否在运营区域内
                ServiceAreaManager areaManager = new ServiceAreaManager();
                Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(vehicleid);
                if (area_ht != null && area_ht.Keys.Count > 0)
                {
                    List<LatLng> area_pts = new List<LatLng>();
                    string coordinates = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates");
                    foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                        area_pts.Add(pt);
                    }
                    string longitude = SiteHelper.GetHashTableValueByKey(vehicle, "longitude");
                    string latitude = SiteHelper.GetHashTableValueByKey(vehicle, "latitude");
                    LatLng vehicle_pt = new LatLng(double.Parse(latitude),double.Parse(longitude));
                    //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                    vehicle_pt = SiteHelper.GPSToGCJ02(longitude,latitude);
                    vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6),Math.Round(vehicle_pt.longitude, 6));
                    bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                    if (!isInPoly)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆预约失败,车辆处于运营区域外");
                    }
                }
                #endregion

                #region 判断车辆是否在停车点内
                /*string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
                Hashtable parking_ht = areaManager.GetNearestParking(SiteHelper.GetHashTableValueByKey(vehicle, "longitude"), SiteHelper.GetHashTableValueByKey(vehicle, "latitude"), vehicleid);
                if (returnVehicleMode == "1" && parking_ht != null && parking_ht.Keys.Count > 0)
                {
                    List<LatLng> parking_pts = new List<LatLng>();
                    string coordinates = SiteHelper.GetHashTableValueByKey(parking_ht, "Coordinates");
                    foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                        parking_pts.Add(pt);
                    }
                    string longitude = SiteHelper.GetHashTableValueByKey(vehicle, "longitude");
                    string latitude = SiteHelper.GetHashTableValueByKey(vehicle, "latitude");
                    LatLng vehicle_pt = new LatLng(double.Parse(latitude),double.Parse(longitude));
                    //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                    vehicle_pt = SiteHelper.GPSToGCJ02(longitude,latitude);
                    vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6),Math.Round(vehicle_pt.longitude, 6));
                    bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, parking_pts);
                    if (!isInPoly)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆预约失败,车辆处于停车网点外");
                    }
                }*/
                #endregion

                //判断车辆是否可用
                if (SiteHelper.GetHashTableValueByKey(vehicle, "UseState").Trim() == VehicleUseState.Free.GetHashCode().ToString() && SiteHelper.GetHashTableValueByKey(vehicle, "VehicleState").Trim() == VehicleState.Use.GetHashCode().ToString())
                {
                    //判断用户余额是否充足
                    Hashtable user = uim.GetUserInfoByUserID(uid);
                    if (user == null || user.Keys.Count <= 0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "您不是有效会员");
                    }
                    else
                    {
                        if ("2".CompareTo(user["REALNAMECERTIFICATION"].ToString()) == 0)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "当前认证正在审核中，请耐心等待");
                        }
                        //验证用户是否经过身份认证
                        if ("4".CompareTo(user["REALNAMECERTIFICATION"].ToString()) != 0)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前未完成实名认证，请完成实名认证后重试");
                        }
                        //验证用户状态是否有效
                        if ("0".CompareTo(user["USERSTATE"].ToString()) == 0)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前已被禁用无法预约车辆，如有问题请与客服联系");
                        }

                        //判断用户是否缴纳押金
                        decimal deposit = 0.00m;
                        decimal.TryParse(user["DEPOSIT"].ToString(), out deposit);
                        if (user["USERTYPE"].ToString() !=UserType.OffDepositUser.GetHashCode().ToString() && deposit <= 0.00m)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前未缴纳用车押金，请完成押金缴纳后重试");
                        }
                    }

                    //判断用户是否存在未支付订单
                    if (om.IsExistUnPayOrderByUser(uid))
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前有未支付订单，请完成支付后重试");
                    }

                    //判断会员是否有条件预约
                    bool isExist = om.IsExistVehicleOrderByUser(uid);
                    if (isExist)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "您已经预约过车辆了");
                    }
                    else
                    {
                        //如果是扫码用车
                        if (res["isopen"] != null && res["isopen"].ToString() == "1")
                        {
                            string gpsnum = SiteHelper.GetHashTableValueByKey(vehicle, "VehicleGPSNum");
                            //开车
                            bool isOpen = vm.OpenVehicle(vehicleid);
                            if (!isOpen)//开车失败
                            {
                                return SiteHelper.GetJsonFromHashTable(null, "faild", "非常抱歉，由于车辆启动失败本次扫码用车失败，请选择其他车辆或稍后重试");
                            }
                            else//开车成功
                            {
                                string orderID = string.Empty;
                                string orderNum = string.Empty;
                                //提交订单
                                bool isSuccess = OrderVehicle(res, vehicle, user, out orderID, out orderNum);
                                if (!isSuccess)
                                {
                                    isOpen = vm.CloseVehicle(vehicleid);
                                    return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆预约失败,请联系管理员");
                                }
                                else
                                {
                                    return SiteHelper.GetJsonFromHashTable(null, "success", "车辆预约成功并已开启，欢迎驾驶电斑马");
                                }
                            }
                        }
                        else //不是扫码用车
                        {
                            string orderID = string.Empty;
                            string orderNum = string.Empty;
                            //提交订单
                            bool isSuccess = OrderVehicle(res, vehicle, user, out orderID, out orderNum);
                            if (isSuccess)
                            {

                                return SiteHelper.GetJsonFromHashTable(null, "success", "车辆预约成功");
                            }
                            else
                            {
                                return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆预约失败,请联系管理员");
                            }

                        }
                    }
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "预约失败,车辆不允许预约");
                }
            }
        }

        private bool OrderVehicle(Hashtable res, Hashtable vehicle, Hashtable user, out string orderID, out string orderNum)
        {
            orderID = CommonHelper.GetGuid;
            orderNum = SiteHelper.GenerateOrderNum();
            bool result = false;
            try
            {
                string km_price = "", minute_price = "";
                VehiclePriceManager vp = new VehiclePriceManager();
                Hashtable price_ht= vp.GetVehiclePrice(vehicleid);
                if(price_ht!=null && price_ht.Keys.Count>0)
                {
                    km_price=price_ht["KmPrice"].ToString();
                    minute_price=price_ht["MinutePrice"].ToString();
                }

                int renttime = 0;
                decimal TotalMoney = 0;
                decimal SettlementMoney = TotalMoney;
                Hashtable param = new Hashtable();
                param["VID"] = vehicleid;
                param["OID"] = orderID;
                param["OrderNum"] = orderNum;
                param["UserID"] = uid;
                param["VehicleID"] = vehicleid;
                param["KMPrice"] = km_price;
                param["MinutePrice"] = minute_price;
                param["Mileage"] = 0;
                param["Minutes"] = 0;
                param["PayState"] = OrderPayState.NotPay.GetHashCode();
                param["PayState"] = OrderPayState.NotPay.GetHashCode();
                param["OrderState"] = OrderState.Valid.GetHashCode();
                param["CreateTime"] = SiteHelper.GetWebServerCurrentTime();
                param["OrderRemark"] = "";
                param["RentDuration"] = renttime;
                param["TotalMoney"] = TotalMoney;
                param["SettlementMoney"] = SettlementMoney;
                param["PayMoney"] = SettlementMoney;
                param["PayTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                param["FinishedTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                param["ChangesAmount"] = -Math.Abs(SettlementMoney);
                param["UserFinancialID"] = CommonHelper.GetGuid;
                param["UserID"] = uid;
                param["ChangesTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                param["ChangesType"] = UserFinancialChangesType.Consumption.GetHashCode().ToString();
                param["Remark"] = "";
                param["Operator"] = "";
                param["OperatorType"] = UserFinancialOperatorType.Admin.GetHashCode().ToString();
                param["OperatorWay"] = UserFinancialOperatorWay.Plat.GetHashCode().ToString();
                param["State"] = UserFinancialState.Effect.GetHashCode().ToString();
                param["MessageID"] = CommonHelper.GetGuid;
                param["CityID"] = SiteHelper.GetHashTableValueByKey(vehicle, "CityID").ToString();
                param["OrderSource"] = "01";
                param["FromParking"] = SiteHelper.GetHashTableValueByKey(vehicle, "ParkingID").ToString();

                UserOrderVehicleManager uovm = new UserOrderVehicleManager();
                result = uovm.OrderVehicle(param);
                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}