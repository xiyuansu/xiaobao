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
    /// 订单分享成功
    /// </summary>
    public class OrderShare : IApiAction2
    {
        private string uid = string.Empty;

        private string orderid = string.Empty;

        private string platform = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["OrderID"] == null || res["OrderID"].ToString().Trim().Length <= 0 ||
                res["Platform"] == null || res["Platform"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "分享数据不完整");
            }
            else
            {
                UserInfoManager uim = new UserInfoManager();
                OrdersManager om = new OrdersManager();
                SysSettingManager settingManager = new SysSettingManager();
                UserCouponManager couponManager = new UserCouponManager();

                uid = res["UID"].ToString().Trim();
                orderid = res["OrderID"].ToString().Trim();
                platform = res["Platform"].ToString().Trim();

                Hashtable order_ht = om.GetOrderInfoByID(orderid);
                if (order_ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到订单数据,分享失败");
                }
                string orderstate = SiteHelper.GetHashTableValueByKey(order_ht, "OrderState");
                if (orderstate != OrderState.Finished.GetHashCode().ToString())
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "订单未完成,分享失败");
                }
                string sharestate = SiteHelper.GetHashTableValueByKey(order_ht, "ShareState");
                bool result = true;
                if(sharestate=="0")
                {
                    int couponNum = 0;
                    int.TryParse(settingManager.GetValueByKey("OrderShareNum"), out couponNum);
                    decimal couponMoney = 0.00m;
                    decimal.TryParse(settingManager.GetValueByKey("OrderShareMoney"), out couponMoney);
                    if (couponNum > 0 && couponMoney > 0)
                    {
                        result = couponManager.OrderShareGiveCoupons(uid, orderid, platform);
                    }
                }
                if (result)
                {
                    //响应数据到客户端
                    return SiteHelper.GetJsonFromHashTable(null, "success", "订单分享成功", "UserInfo");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "订单分享失败");
                }
            }
        }

    }
}