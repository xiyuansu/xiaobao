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
    public class AlipayTransfer : IApiAction2
    {
        //用户id
        private string uid = string.Empty;
        private Hashtable res = null;

        private string trade_no;
        private string buyer_id;

        private static Log Logger = LogFactory.GetLogger(typeof(AlipayQuery));

        public string Execute(Hashtable params_ht)
        {
            //AlipayTransfer  TradeNo=2017121121001004470568843689&BuyerID=2088002216117471
            Hashtable res = params_ht;

            if (res["TradeNo"] == null || res["TradeNo"].ToString().Trim().Length <= 0
                || res["BuyerID"] == null || res["BuyerID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {

                trade_no = res["TradeNo"].ToString().Trim();
                buyer_id = res["BuyerID"].ToString().Trim();
                string errmsg = "";
                AlipayRefund alipay = new AlipayRefund();
                bool result = alipay.Transfer(trade_no, buyer_id, 10m, out errmsg);
                if (result)
                {
                    return "{\"result\":\"退款成功\",\"message\":\""+ errmsg + "\"}";
                }
                else
                {
                    return "{\"result\":\"退款失败\",\"message\":\"" + errmsg + "\"}";
                }
            }
        }
    }
}