using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 关闭车辆
    /// </summary>
    public class CloseVehicle : IApiAction2
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
                Hashtable order = om.GetUserCurrentOrderVehicle(uid);
                Hashtable ht = om.GetUserCurrentOrderVehicleGPSNum(uid);
                if (ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您没有车辆无法锁车");
                }
                else
                {
                    //检测车辆当前开关状态,开:unlock，关:lock
                    if ("lock".CompareTo(SiteHelper.GetHashTableValueByKey(order, "LockState")) == 0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "已临时锁车,如需还车请点击还车,以免额外计费");
                    }
                    string vid = SiteHelper.GetHashTableValueByKey(ht, "VID");
                    bool isSuccess = vm.CloseVehicle(vid);
                    if (!isSuccess)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "临时锁车失败，请稍后重试");
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "success", "已临时锁车,如需还车请点击还车,以免额外计费");
                    }
                }
            }
        }
    }
}