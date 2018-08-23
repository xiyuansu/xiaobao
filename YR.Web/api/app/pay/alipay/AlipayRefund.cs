using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;

namespace YR.Web.api.app.pay.alipay
{
    /// <summary>
    /// 退款接口
    /// </summary>
    public class AlipayRefund
    {
        private static Log Logger = LogFactory.GetLogger(typeof(AlipayRefund));

        private const string https_url = "https://openapi.alipay.com/gateway.do";

        private const string method = "alipay.trade.refund";

        //private const string sign_type = "RSA";
        private const string sign_type = "RSA2";

        private const string charset = "utf-8";

        public AlipayRefund()
        {

        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        private static string CreateLinkString(SortedDictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        private static string CreateLinkStringUrlencode(SortedDictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" +HttpUtility.UrlEncode(temp.Value) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 退款操作
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <param name="refund_amount"></param>
        /// <param name="refund_reason"></param>
        /// <returns></returns>
        public bool Refund2(string trade_no, decimal refund_amount, out string errmsg,out string code)
        {
            errmsg = "";
            code = "";
            bool result = false;
            try
            {
                IAopClient client = new DefaultAopClient(https_url, Config.app_id, Config.private_key, "json", "1.0", sign_type, Config.alipay_public_key, charset, false);
                AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
                request.BizContent = "{" +
                "\"trade_no\":\""+ trade_no + "\"," +
                "\"refund_amount\":"+ refund_amount.ToString("N2") + "," +
                "\"refund_reason\":\"押金正常退款\"," +
                "\"body\":\"押金退款\"" +
                "  }";
                AlipayTradeRefundResponse response = client.Execute(request);
                code = response.Code;
                if ("10000".Equals(code))
                {
                    errmsg = "退款成功";
                    result = true;
                }
                else
                {
                    errmsg = response.SubMsg;
                }
                //支付宝押金退款失败(2018041021001004160529711545):40004交易已结束
                Logger.Warn(string.Format("支付宝押金退款{0}({1}):{2},{3}",result?"成功":"失败", trade_no,code,errmsg));
                return result;
            }
            catch(Exception ex)
            {
                Logger.Warn(string.Format("支付宝押金退款失败({0}):{1}",trade_no,ex.Message));
                return false;
            }
        }

        public bool Transfer(string trade_no,string buyer_id,decimal refund_amount,out string errmsg)
        {
            errmsg = "";
            string code = "";
            bool result = false;
            try
            {
                StringBuilder biz_content = new StringBuilder();
                biz_content.Append(@"{");
                biz_content.AppendFormat("\"out_biz_no\":\"{0}\",", trade_no);
                biz_content.AppendFormat("\"payee_type\":\"{0}\",", "ALIPAY_USERID");
                biz_content.AppendFormat("\"payee_account\":\"{0}\",", buyer_id);
                biz_content.AppendFormat("\"amount\":\"{0}\",", refund_amount.ToString("N2"));
                biz_content.AppendFormat("\"payer_show_name\":\"{0}\",", "小宝出行押金退款");
                biz_content.AppendFormat("\"remark\":\"{0}\"", "押金转账退款");
                biz_content.Append(@"}");
                SortedDictionary<string, string> dicArray = new SortedDictionary<string, string>();
                dicArray.Add("app_id", Config.app_id);
                dicArray.Add("biz_content", biz_content.ToString());
                dicArray.Add("charset", charset);
                dicArray.Add("format", "JSON");
                dicArray.Add("method", "alipay.fund.trans.toaccount.transfer");
                dicArray.Add("sign_type", sign_type);
                dicArray.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dicArray.Add("version", "1.0");

                string sign_content = CreateLinkString(dicArray);
                string sign = RSA.sign(sign_content, Config.private_key, "UTF-8", sign_type);
                dicArray.Add("sign", sign);

                string postDataStr = CreateLinkStringUrlencode(dicArray);
                string responseString = HttpUtil.Get(https_url, postDataStr, "utf-8");

                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(responseString);
                string alipay_fund_trans_toaccount_transfer_response = jsonObj["alipay_fund_trans_toaccount_transfer_response"].ToString();

                Dictionary<string, string> dict1 = new Dictionary<string, string>();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                dict1 = jss.Deserialize<Dictionary<string, string>>(alipay_fund_trans_toaccount_transfer_response);
                //if (GetSignVeryfy(dict1, jsonObj["sign"].ToString())){
                AlipayRefundResponse refund_response = Newtonsoft.Json.JsonConvert.DeserializeObject<AlipayRefundResponse>(alipay_fund_trans_toaccount_transfer_response);
                code = refund_response.code;
                if (refund_response.code == "10000")
                {
                    result = true;
                }
                errmsg = refund_response.sub_msg;
                //}
                Logger.Warn(string.Format("支付宝押金转账退款{0}({1}):{2}{3}", result ? "成功" : "失败", trade_no,code, errmsg));
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("支付宝押金转账退款失败({0}):{1}", trade_no, ex.Message));
                return false;
            }
        }

        private static string CreateJsonString(Dictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append("\""+temp.Key + "\":\"" + temp.Value + "\",");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 获取返回时的签名验证结果
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="sign">对比的签名结果</param>
        /// <returns>签名验证结果</returns>
        public bool GetSignVeryfy(Dictionary<string, string> inputPara, string sign)
        {
            Dictionary<string, string> sPara = new Dictionary<string, string>();

            //过滤空值、sign与sign_type参数
            sPara = Core.FilterPara(inputPara);

            //获取待签名字符串
            string preSignStr = CreateJsonString(sPara);
            preSignStr= "{" + preSignStr + "}";
            //获得签名验证结果
            bool isSgin = false;
            if (sign != null && sign != "")
            {
                switch (sign_type)
                {
                    case "RSA":
                        isSgin = RSA.verify(preSignStr, sign, Config.alipay_public_key, charset);
                        break;
                    case "RSA2":
                        isSgin = RSA.verify2(inputPara, Config.alipay_public_key, charset, sign_type, false);
                        break;
                    default:
                        break;
                }
            }
            return isSgin;
        }
    }

    public class AlipayRefundResponse
    {
        public string code;

        public string msg;

        public string sub_code;

        public string sub_msg;
    }
}