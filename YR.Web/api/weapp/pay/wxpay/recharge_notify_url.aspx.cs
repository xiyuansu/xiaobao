using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections.Generic;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetData;
using Asiasofti.SmartVehicle.Common.Enum;
using System.Text;
using System.IO;
using Deepleo.Weixin.SDK.Pay;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;


namespace YR.Web.api.weapp.pay.wxpay
{
    public partial class recharge_notify_url : System.Web.UI.Page
    {
        private static Log Logger = LogFactory.GetLogger(typeof(recharge_notify_url));

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            StreamReader reader = new StreamReader(Request.InputStream);
            String xmlData = reader.ReadToEnd();
            var sPara = GetRequestPostByXml(xmlData);
            if (sPara.Count <= 0)
            {
                throw new ArgumentNullException();
            }

            if (sPara["return_code"] == "SUCCESS" && sPara["result_code"] == "SUCCESS")
            {
                var sign = sPara["sign"];

                Dictionary<string, string> sPara2 = new Dictionary<string, string>();
                foreach (var item in sPara)
                {
                    if (item.Key != "sign")
                        sPara2.Add(item.Key, item.Value);
                }

                var signValue = WxPayAPI.Sign(sPara2, WxConfig.PartnerKey);
                bool isVerify = sign == signValue;
                if (isVerify)
                {
                    var out_trade_no = sPara["out_trade_no"];
                    var transaction_id = sPara["transaction_id"];
                    var total_fee = sPara["total_fee"];

                    decimal pay_money = decimal.Parse(total_fee) / 100;
                    Logger.Info(string.Format("用户充值,微信公众号支付通知,订单号{0},支付单号{1},支付金额{2}", out_trade_no, transaction_id, pay_money));

                    UserInfoManager uim = new UserInfoManager();
                    UserFinancialManager ufm = new UserFinancialManager();
                    DataTable userfinancial = ufm.GetUserFinancialInfoByPayId(out_trade_no);
                    if (userfinancial != null && userfinancial.Rows.Count == 1)
                    {
                        Hashtable hashufdb = DataTableHelper.DataRowToHashTable(userfinancial.Rows[0]);
                        Hashtable hashuf = new Hashtable();
                        hashuf["ID"] = SiteHelper.GetHashTableValueByKey(hashufdb, "ID");
                        hashuf["UserID"] = SiteHelper.GetHashTableValueByKey(hashufdb, "UserID");
                        hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                        hashuf["TradeNo"] = transaction_id;
                        hashuf["TotalFee"] = pay_money;
                        hashuf["PayWay"] = UserFinancialOperatorWay.WeixinPubPay;
                        decimal changesAmount = 0.00m;
                        decimal.TryParse(SiteHelper.GetHashTableValueByKey(hashufdb, "ChangesAmount"), out changesAmount);
                        bool isSuccess = false;
                        if (Math.Abs(changesAmount) == pay_money)
                            isSuccess = uim.RechargeCallBack(hashuf);
                        if (isSuccess)
                        {
                            sb.Append("<xml>");
                            sb.Append("<return_code>SUCCESS</return_code>");
                            sb.Append("<return_msg></return_msg>");
                            sb.Append("</xml>");
                        }
                        else
                        {
                            sb.Append("<xml>");
                            sb.Append("<return_code>FAIL</return_code>");
                            sb.Append("<return_msg>error</return_msg>");
                            sb.Append("</xml>");
                        }
                    }
                }
                else
                {
                    sb.Append("<xml>");
                    sb.Append("<return_code>FAIL</return_code>");
                    sb.Append("<return_msg>error</return_msg>");
                    sb.Append("</xml>");
                }
            }
            else
            {
                sb.Append("<xml>");
                sb.Append("<return_code>FAIL</return_code>");
                sb.Append("<return_msg>error</return_msg>");
                sb.Append("</xml>");
            }

            Response.Write(sb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetRequestPostByXml(string xmlString)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            document.XmlResolver = null;
            document.LoadXml(xmlString);

            var nodes = document.ChildNodes[0].ChildNodes;

            foreach (System.Xml.XmlNode item in nodes)
            {
                dic.Add(item.Name, item.InnerText);
            }
            return dic;
        }

        /// <summary>
        /// 并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost(NameValueCollection form)
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();

            // Get names of all forms into a string array.
            String[] requestItem = form.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], HttpUtility.UrlDecode(form[requestItem[i]], Encoding.UTF8));
            }

            return sArray;
        }

    }
}