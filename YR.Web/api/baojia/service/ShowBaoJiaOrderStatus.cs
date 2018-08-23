using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;

namespace YR.Web.api.baojia.service
{
    /// <summary>
    /// 车辆下单、还车、故障订单终止
    /// </summary>
    public class ShowBaoJiaOrderStatus : IApiAction
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
            int order_status = int.Parse(params_ht["order_status"].ToString());
            int status = int.Parse(params_ht["status"].ToString());
            string mobile = params_ht["mobile"].ToString();
            string user_name = params_ht["user_name"].ToString();
            string card_no = params_ht["card_no"].ToString();
            string card_pic_url = "";
            if (params_ht["card_pic_url"] != null)
                card_pic_url = params_ht["card_pic_url"].ToString();
            string fault_type = "";
            if (params_ht["fault_type"] != null)
                fault_type = params_ht["fault_type"].ToString();

            if (plate_no != null && (plate_no.StartsWith("http://") || plate_no.StartsWith("https://")))
            {
                //http://api.qingniaokeji.cn/download/app.aspx?no=xxxxxxxxxxxxx
                Uri uri = new Uri(plate_no);
                System.Collections.Specialized.NameValueCollection coll = System.Web.HttpUtility.ParseQueryString(uri.Query);
                plate_no = coll.Get("no");
            }

            bool result = false;
            string errmsg = "";
            switch (order_status)
            {
                case 10000:
                    result = CreateOrder(order_no, plate_no, user_name, mobile, card_no, out errmsg);
                    break;
                case 10301:
                    result = CancelOrder(order_no, fault_type, out errmsg);
                    break;
                case 80200:
                    result = ReturnVehicle(order_no, out errmsg);
                    break;
                default:
                    break;
            }
            if (result)
            {
                resp.code = "0";
                resp.msg = "成功";
            }
            else
            {
                resp.code = "-1";
                resp.msg = errmsg;
            }
            return resp;
        }

        /// <summary>
        /// 车辆下单
        /// </summary>
        /// <param name="out_order_no"></param>
        /// <param name="plate_no"></param>
        /// <param name="username"></param>
        /// <param name="mobile"></param>
        /// <param name="cardno"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool CreateOrder(string out_order_no, string plate_no, string username, string mobile, string cardno, out string errmsg)
        {
            VehicleManager vm = new VehicleManager();
            BaoJiaManager mgr = new BaoJiaManager();
            SysSettingManager settingManager = new SysSettingManager();
            Hashtable vehicle = vm.GetVehicleInfoByID(plate_no);
            if (vehicle == null || vehicle.Keys.Count == 0)
            {
                //根据车牌号码查询车辆信息
                vehicle = vm.GetVehicleInfoByLicenseNumber(plate_no);
            }
            //判断车辆是否存在
            if (vehicle == null || vehicle.Keys.Count == 0)
            {
                errmsg = "车辆预约失败,未找到车辆相关信息";
                return false;
            }

            string vehicleid = SiteHelper.GetHashTableValueByKey(vehicle, "ID");
            //判断预约车辆是否离线
            DateTime lastUpdateTime;
            if (string.IsNullOrEmpty(SiteHelper.GetHashTableValueByKey(vehicle, "LastUpdateTime")))
            {
                errmsg = "当前车辆已离线无法约车";
                return false;
            }
            else
            {
                lastUpdateTime = DateTime.Parse(SiteHelper.GetHashTableValueByKey(vehicle, "LastUpdateTime"));
                if (SiteHelper.ExecDateDiff(lastUpdateTime, DateTime.Now) > 10)
                {
                    errmsg = "当前车辆已离线无法约车";
                    return false;
                }
            }

            //判断预约车辆电量值
            double electricity = 0.00;
            double.TryParse(SiteHelper.GetHashTableValueByKey(vehicle, "Electricity"), out electricity);
            if (electricity < 20)
            {
                errmsg = "当前车辆电量过低无法约车";
                return false;
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
                vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
                vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                if (!isInPoly)
                {
                    errmsg = "车辆预约失败,车辆处于运营区域外";
                    return false;
                }
            }
            #endregion

            #region 判断车辆是否在停车点内
            string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
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
                vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
                vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, parking_pts);
                if (!isInPoly)
                {
                    errmsg = "车辆预约失败,车辆处于停车网点外";
                    return false;
                }
            }
            #endregion

            //判断车辆是否可用
            if (SiteHelper.GetHashTableValueByKey(vehicle, "UseState").Trim() == VehicleUseState.Free.GetHashCode().ToString() && SiteHelper.GetHashTableValueByKey(vehicle, "VehicleState").Trim() == VehicleState.Use.GetHashCode().ToString())
            {
                //判断用户是否存在未支付订单
                if (mgr.IsExistUnPayOrderByUser(mobile))
                {
                    errmsg = "您当前有未支付订单，请完成支付后重试";
                    return false;
                }

                //判断会员是否有条件预约
                bool isExist = mgr.IsExistVehicleOrderByUser(mobile);
                if (isExist)
                {
                    errmsg = "您已经预约过车辆了";
                    return false;
                }
                else
                {
                    string orderID = string.Empty;
                    string orderNum = string.Empty;
                    //提交订单
                    bool isSuccess = OrderVehicle(out_order_no, username, mobile, cardno, vehicleid, out orderID, out orderNum);
                    if (isSuccess)
                    {
                        //vm.OpenVehicle(vehicleid);
                        errmsg = "";
                        return true;
                    }
                    else
                    {
                        errmsg = "车辆预约失败,请联系管理员";
                        return false;
                    }
                }
            }
            else
            {
                errmsg = "预约失败,车辆不允许预约";
                return false;
            }
        }

        private bool OrderVehicle(string out_order_no, string username, string mobile, string cardno, string vehicleid, out string orderID, out string orderNum)
        {
            orderID = CommonHelper.GetGuid;
            orderNum = SiteHelper.GenerateOrderNum();
            bool result = false;
            try
            {
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle = vm.GetVehicleInfoByID(vehicleid);

                string km_price = "", minute_price = "";
                VehiclePriceManager vp = new VehiclePriceManager();
                Hashtable price_ht = vp.GetVehiclePrice(vehicleid);
                if (price_ht != null && price_ht.Keys.Count > 0)
                {
                    km_price = price_ht["KmPrice"].ToString();
                    minute_price = price_ht["MinutePrice"].ToString();
                }

                int renttime = 0;
                decimal TotalMoney = 0;
                decimal SettlementMoney = TotalMoney;
                Hashtable param = new Hashtable();
                param["VID"] = vehicleid;
                param["OID"] = orderID;
                param["OrderNum"] = orderNum;
                param["UserID"] = null;
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
                param["ChangesTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                param["ChangesType"] = UserFinancialChangesType.Consumption.GetHashCode().ToString();
                param["Remark"] = "";
                param["Operator"] = "";
                param["OperatorType"] = UserFinancialOperatorType.Admin.GetHashCode().ToString();
                param["OperatorWay"] = UserFinancialOperatorWay.Plat.GetHashCode().ToString();
                param["State"] = UserFinancialState.Effect.GetHashCode().ToString();
                param["MessageID"] = CommonHelper.GetGuid;
                param["CityID"] = SiteHelper.GetHashTableValueByKey(vehicle, "CityID").ToString();
                param["RealName"] = username;
                param["BindPhone"] = mobile;
                param["IDCardNum"] = cardno;
                param["OutOrderNo"] = out_order_no;
                param["OrderSource"] = "02";
                param["FromParking"] = SiteHelper.GetHashTableValueByKey(vehicle, "ParkingID").ToString();

                BaoJiaManager mgr = new BaoJiaManager();
                result = mgr.OrderVehicle(param);
                return result;
            }
            catch
            {
                return result;
            }
        }

        /// <summary>
        /// 还车
        /// </summary>
        /// <param name="order_no"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool ReturnVehicle(string order_no, out string errmsg)
        {
            BaoJiaManager mgr = new BaoJiaManager();

            VehicleManager vm = new VehicleManager();
            ServiceAreaManager areaManager = new ServiceAreaManager();
            SysSettingManager settingManager = new SysSettingManager();
            Hashtable order = mgr.GetOrderInfo(order_no);
            //判断是否有车
            if (order == null)
            {
                errmsg = "未找到相关订单";
                return false;
            }
            else
            {
                string vid = SiteHelper.GetHashTableValueByKey(order, "VehicleID");
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vid);
                string longitude = SiteHelper.GetHashTableValueByKey(vehicle_ht, "Longitude");
                string latitude = SiteHelper.GetHashTableValueByKey(vehicle_ht, "Latitude");

                //处理订单
                string order_state = SiteHelper.GetHashTableValueByKey(order, "OrderState");
                if (order_state != "1")
                {
                    errmsg = "订单当前所在状态无法还车";
                    return false;
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
                    //decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "Minutes"), out minutes);
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

                    string gpsnum = SiteHelper.GetHashTableValueByKey(order, "VehicleGPSNum");
                    //锁车
                    bool isSuccess = vm.CloseVehicle(SiteHelper.GetHashTableValueByKey(order, "VehicleID"));
                    if (!isSuccess)
                    {
                        errmsg = "锁车失败，还车操作失败";
                        return false;
                    }
                    VehiclePriceManager priceManager = new VehiclePriceManager();
                    Hashtable billing_ht = priceManager.GetOrderSettlement(SiteHelper.GetHashTableValueByKey(order, "ID"));
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "MinutePrice"), out minuteprice);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "KmPrice"), out kmprice);
                    int.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "Minutes"), out minutes);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "TotalMoney"), out TotalMoney);

                    SettlementMoney = TotalMoney;

                    #region 判断还车点是否在运营区域内，运营区域外加收费用
                    Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(SiteHelper.GetHashTableValueByKey(order, "VehicleID"));
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
                        LatLng vehicle_pt = new LatLng(double.Parse(latitude), double.Parse(longitude));
                        //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                        vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
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
                        LatLng vehicle_pt = new LatLng(double.Parse(latitude), double.Parse(longitude));
                        //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                        vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
                        vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                        Hashtable parking_ht = areaManager.GetNearestParking(vehicle_pt.longitude.ToString(), vehicle_pt.latitude.ToString(), SiteHelper.GetHashTableValueByKey(order, "VehicleID"));

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
                        }
                    }
                    #endregion

                    payMoney = TotalMoney;

                    Hashtable userParm = new Hashtable();
                    userParm["VID"] = SiteHelper.GetHashTableValueByKey(order, "VehicleID");
                    userParm["OrderNum"] = SiteHelper.GetHashTableValueByKey(order, "OrderNum");
                    userParm["TotalMoney"] = TotalMoney;
                    userParm["SettlementMoney"] = SettlementMoney;
                    userParm["PayMoney"] = payMoney;
                    userParm["PayTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["EndTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["FinishedTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["OID"] = SiteHelper.GetHashTableValueByKey(order, "id");
                    userParm["ChangesAmount"] = -Math.Abs(TotalMoney);
                    userParm["UserFinancialID"] = CommonHelper.GetGuid;
                    userParm["UserID"] = null;
                    userParm["ChangesTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["ChangesType"] = UserFinancialChangesType.Consumption.GetHashCode().ToString();
                    userParm["OrderID"] = SiteHelper.GetHashTableValueByKey(order, "id");
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

                    bool isSettlementSuccess = mgr.ReturnVehicle(userParm);
                    if (isSettlementSuccess)
                    {
                        OrdersManager om = new OrdersManager();
                        om.UpdateOrderGPS(SiteHelper.GetHashTableValueByKey(order, "id"), false);
                        errmsg = "还车成功";
                        return true;
                    }
                    else
                    {
                        errmsg = "锁车操作失败";
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="order_no"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool CancelOrder(string order_no, string fault_type, out string errmsg)
        {
            bool result = false;
            errmsg = "";
            VehicleManager vm = new VehicleManager();
            BaoJiaManager mgr = new BaoJiaManager();
            Hashtable order_ht = mgr.GetOrderInfo(order_no);
            if (order_ht != null && order_ht.Keys.Count > 0)
            {
                string order_state = SiteHelper.GetHashTableValueByKey(order_ht, "OrderState");
                if (order_state == "1")
                {
                    string orderid = SiteHelper.GetHashTableValueByKey(order_ht, "ID");
                    string vid = SiteHelper.GetHashTableValueByKey(order_ht, "VehicleID");
                    vm.CloseVehicle(vid);
                    result = mgr.CancelOrder(orderid);
                }
            }
            return result;
        }
    }

}