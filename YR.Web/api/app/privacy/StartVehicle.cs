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
    /// 用户启动车辆
    /// </summary>
    public class StartVehicle : IApiAction2
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
                VehicleManager vm = new VehicleManager();
                UserInfoManager uim = new UserInfoManager();
                Hashtable order = om.GetUserCurrentOrderVehicle(uid);
                if (order == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您没有车辆无法启动");
                }
                else
                {
                    string vehicleid = order["VID"].ToString();

                    //检测车辆当前开关状态,开:unlock，关:lock
                    if ("unlock".CompareTo(SiteHelper.GetHashTableValueByKey(order, "LockState")) == 0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "当前车辆已处于开锁状态，无需重复开锁");
                    }

                    string gpsnum = SiteHelper.GetHashTableValueByKey(order, "VehicleGPSNum");
                    bool isSuccess = vm.OpenVehicle(vehicleid);
                    //如果是首次开车订单StartTime设为当前时间 12:28 不判断开锁成功
                    if (string.IsNullOrEmpty(order["STARTTIME"].ToString()))
                    {
                        om.SetCurrentOrderStartTime(uid);
                        om.UpdateOrderGPS(order["ORDERID"].ToString(), true);
                    }
                    return SiteHelper.GetJsonFromHashTable(null, "success", "车辆启动指令已发送");
                    /*
                    if (!isSuccess)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆启动失败，请稍后重试");
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "success", "车辆启动成功");
                    }*/
                }
            }
        }

    }
}