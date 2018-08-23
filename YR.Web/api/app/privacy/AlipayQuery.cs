using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
    /// 支付宝订单查询
    /// </summary>
    public class AlipayQuery : IApiAction2
    {
        //用户id
        private string uid = string.Empty;
        private Hashtable res = null;

        private string order_payid;

        private static Log Logger = LogFactory.GetLogger(typeof(AlipayQuery));

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["OrderPayID"] == null || res["OrderPayID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                order_payid = res["OrderPayID"].ToString().Trim();

                AlipayOrderQuery orderQuery = new AlipayOrderQuery();
                //OrderQueryResult queryResult = orderQuery.Query(order_payid);
                OrderQueryResult queryResult = orderQuery.QueryByTradeNO(order_payid);
                string result= JsonConvert.SerializeObject(queryResult);
                return result;
            }
        }
    }
}