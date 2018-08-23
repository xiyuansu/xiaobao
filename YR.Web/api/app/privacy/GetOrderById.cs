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
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取订单详情信息
    /// </summary>
    public class GetOrderById : IApiAction2
    {
        private string uid = string.Empty;

        private string ordernum = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["OrderNum"] == null || res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 || res["OrderNum"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                ordernum = res["OrderNum"].ToString().Trim();

                OrdersManager om = new OrdersManager();
                Hashtable ht = om.GetOrderByNum(ordernum);
                if (ht != null)
                {
                    return SiteHelper.GetJsonFromHashTable(ht, "success", "获取订单数据成功", "OrderInfo");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到相关订单数据");
                }
            }
        }
    }
}