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
    /// 取消订单
    /// </summary>
    public class CancelOrder : IApiAction2
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
                Hashtable order = om.GetUserCurrentOrderVehicle(uid);
                Hashtable ht = om.GetUserCurrentOrderVehicleGPSNum(uid);
                if (ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您没有需要取消的订单");
                }
                else
                {
                    string orderNum = SiteHelper.GetHashTableValueByKey(ht, "OrderNum");
                    string startTime = SiteHelper.GetHashTableValueByKey(ht, "StartTime");
                    if (string.IsNullOrEmpty(startTime))
                    {
                        bool isSuccess = om.CancelOrder(orderNum, 0);
                        if (!isSuccess)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "取消订单失败，请稍后重试");
                        }
                        else
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "success", "取消订单成功");
                        }
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "当前订单不能自动取消");
                    }
                }
            }
        }
    }
}