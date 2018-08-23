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
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.pay.alipay
{
    /// <summary>
    /// 支付订单查询
    /// </summary>
    public class AlipayOrderQuery
    {
        private static Log Logger = LogFactory.GetLogger(typeof(AlipayRefund));

        private const string https_url = "https://openapi.alipay.com/gateway.do";

        private const string method = "alipay.trade.query";

        //private const string sign_type = "RSA";
        private const string sign_type = "RSA2";

        private const string charset = "utf-8";

        public AlipayOrderQuery()
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
        /// 查询订单
        /// </summary>
        /// <param name="trade_no"></param>
        /// <returns></returns>
        public OrderQueryResult QueryByTradeNO(string trade_no)
        {
            OrderQueryResult result = new OrderQueryResult();
            try
            {
                StringBuilder biz_content = new StringBuilder();
                biz_content.Append(@"{");
                biz_content.AppendFormat("\"trade_no\":\"{0}\"", trade_no);
                biz_content.Append(@"}");

                SortedDictionary<string, string> dicArray = new SortedDictionary<string, string>();
                dicArray.Add("app_id", Config.app_id);
                dicArray.Add("biz_content", biz_content.ToString());
                dicArray.Add("charset", charset);
                dicArray.Add("format", "JSON");
                dicArray.Add("method", method);
                dicArray.Add("sign_type", sign_type);
                dicArray.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dicArray.Add("version", "1.0");
                //dicArray.Add("sign", );

                string sign_content = CreateLinkString(dicArray);
                string sign = RSA.sign(sign_content, Config.private_key, "UTF-8", sign_type);
                dicArray.Add("sign", sign);

                string postDataStr = CreateLinkStringUrlencode(dicArray);
                string responseString = HttpUtil.Get(https_url, postDataStr, "utf-8");

                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(responseString);
                string alipay_trade_query_response = jsonObj["alipay_trade_query_response"].ToString();

                Dictionary<string, string> dict1 = new Dictionary<string, string>();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                dict1 = jss.Deserialize<Dictionary<string, string>>(alipay_trade_query_response);
                Notify aliNotify = new Notify();
                if (aliNotify.GetSignVeryfy(dict1, jsonObj["sign"].ToString()))
                {
                    AlipayQueryResponse query_response = Newtonsoft.Json.JsonConvert.DeserializeObject<AlipayQueryResponse>(alipay_trade_query_response);
                    if (query_response.code == "10000")
                    {
                        result.out_trade_no = query_response.out_trade_no;
                        result.total_fee = query_response.total_amount;
                        result.transaction_id = query_response.trade_no;
                        result.buyer_logon_id = query_response.buyer_logon_id;
                        result.buyer_user_id = query_response.buyer_user_id;
                        string trade_state = query_response.trade_status;
                        result.trade_state = TradeStateEnum.OTHERS;
                        switch (trade_state)
                        {
                            case "TRADE_SUCCESS":
                                result.trade_state = TradeStateEnum.SUCCESS;
                                break;
                            case "WAIT_BUYER_PAY":
                                result.trade_state = TradeStateEnum.NOTPAY;
                                break;
                            case "TRADE_CLOSED":
                                result.trade_state = TradeStateEnum.CLOSED;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (query_response.code == "40004")
                    {
                        if (query_response.sub_code == "ACQ.TRADE_NOT_EXIST")
                            result.trade_state = TradeStateEnum.NOTEXIST;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <returns></returns>
        public OrderQueryResult Query(string out_trade_no)
        {
            OrderQueryResult result = new OrderQueryResult();            
            try
            {
                StringBuilder biz_content = new StringBuilder();
                biz_content.Append(@"{");
                biz_content.AppendFormat("\"out_trade_no\":\"{0}\"", out_trade_no);
                biz_content.Append(@"}");

                SortedDictionary<string, string> dicArray = new SortedDictionary<string, string>();
                dicArray.Add("app_id", Config.app_id);
                dicArray.Add("biz_content", biz_content.ToString());
                dicArray.Add("charset", charset);
                dicArray.Add("format", "JSON");
                dicArray.Add("method", method);
                dicArray.Add("sign_type", sign_type);
                dicArray.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dicArray.Add("version", "1.0");
                //dicArray.Add("sign", );

                string sign_content = CreateLinkString(dicArray);
                string sign = RSA.sign(sign_content, Config.private_key, "UTF-8", sign_type);
                dicArray.Add("sign", sign);

                string postDataStr = CreateLinkStringUrlencode(dicArray);
                string responseString = HttpUtil.Get(https_url, postDataStr, "utf-8");

                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(responseString);
                string alipay_trade_query_response = jsonObj["alipay_trade_query_response"].ToString();
                
                Dictionary<string, string> dict1 = new Dictionary<string, string>();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                dict1 = jss.Deserialize<Dictionary<string, string>>(alipay_trade_query_response);
                Notify aliNotify = new Notify();
                if (aliNotify.GetSignVeryfy(dict1, jsonObj["sign"].ToString()))
                {
                    AlipayQueryResponse query_response = Newtonsoft.Json.JsonConvert.DeserializeObject<AlipayQueryResponse>(alipay_trade_query_response);
                    if (query_response.code == "10000")
                    {
                        result.out_trade_no = query_response.out_trade_no;
                        result.total_fee = query_response.total_amount;
                        result.transaction_id = query_response.trade_no;
                        result.buyer_logon_id = query_response.buyer_logon_id;
                        result.buyer_user_id = query_response.buyer_user_id;
                        string trade_state = query_response.trade_status;
                        result.trade_state = TradeStateEnum.OTHERS;
                        switch (trade_state)
                        {
                            case "TRADE_SUCCESS":
                                result.trade_state = TradeStateEnum.SUCCESS;
                                break;
                            case "WAIT_BUYER_PAY":
                                result.trade_state = TradeStateEnum.NOTPAY;
                                break;
                            case "TRADE_CLOSED":
                                result.trade_state = TradeStateEnum.CLOSED;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (query_response.code == "40004")
                    {
                        if(query_response.sub_code=="ACQ.TRADE_NOT_EXIST")
                            result.trade_state = TradeStateEnum.NOTEXIST;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
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
        /*public bool GetSignVeryfy(Dictionary<string, string> inputPara, string sign)
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
                        isSgin = RSA.verify2(inputPara, Config.alipay_public_key, charset,sign_type, false);
                        break;
                    default:
                        break;
                }
            }

            return isSgin;
        }*/

    }

    public class AlipayQueryResponse
    {
        public string code;

        public string msg;

        public string sub_code;

        public string sub_msg;
        //支付宝交易号	2013112011001004330000121536
        public string trade_no;
        //商家订单号	6823789339978248
        public string out_trade_no;
        //买家支付宝账号	159****5620
        public string buyer_logon_id;
        //买家在支付宝的用户id	2088101117955611
        public string buyer_user_id;
        //交易状态：WAIT_BUYER_PAY（交易创建，等待买家付款）、TRADE_CLOSED（未付款交易超时关闭，或支付完成后全额退款）、TRADE_SUCCESS（交易支付成功）、TRADE_FINISHED（交易结束，不可退款）
        public string trade_status;
        //交易的订单金额
        public decimal total_amount;
        //实收金额
        public decimal receipt_amount;
        //买家实付金额
        public decimal buyer_pay_amount;
    }
}