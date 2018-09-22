using System;
using System.Collections.Generic;
using System.Text;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;

namespace YR.Web.api.app.pay.wxpay
{
    /// <summary>
    /// 支付订单查询
    /// </summary>
    public class WxOrderQuery
    {
        private static Log Logger = LogFactory.GetLogger(typeof(WxOrderQuery));

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <returns></returns>
        public string genNonceStr()
        {
            int length = 16;
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = "";
            Random rad = new Random();
            for (int i = 0; i < length; i++)
            {
                str += chars.Substring(rad.Next(0, chars.Length - 1), 1);
            }
            return str;
        }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns></returns>
        public string genTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        ///获取字符串的md5值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string GetMD5(string text)
        {
            string result = "";
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
            data = md5.ComputeHash(data);
            for (int i = 0; i < data.Length; i++)
            {
                result += data[i].ToString("x2");
            }
            return result;
        }

        /// <summary>
        /// 获取支付参数签名
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getSign(SortedDictionary<string, string> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> item in dic)
            {
                sb.Append(item.Key);
                sb.Append('=');
                sb.Append(item.Value);
                sb.Append('&');
            }
            sb.Append("key=");
            sb.Append(WxConfig.PartnerKey);
            String packageSign = GetMD5(sb.ToString()).ToUpper();
            return packageSign;
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
                SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                dic.Add("appid", pay.wxpay.WxConfig.AppId);
                dic.Add("mch_id", pay.wxpay.WxConfig.MchId);
                dic.Add("out_trade_no", out_trade_no);
                dic.Add("nonce_str", genNonceStr());

                string get_sign = getSign(dic);

                string url = "https://api.mch.weixin.qq.com/pay/orderquery";
                string _req_data = "<xml>";
                _req_data += "<appid>" + dic["appid"] + "</appid>";
                _req_data += "<mch_id><![CDATA[" + dic["mch_id"] + "]]></mch_id> ";
                _req_data += "<out_trade_no><![CDATA[" + dic["out_trade_no"] + "]]></out_trade_no> ";
                _req_data += "<nonce_str><![CDATA[" + dic["nonce_str"] + "]]></nonce_str> ";
                _req_data += "<sign><![CDATA[" + get_sign + "]]></sign> ";
                _req_data += "</xml>";

                string xml = HttpUtil.Post(url, _req_data, "utf-8");
                dynamic data = new DynamicXml(xml);

                if ("SUCCESS" == data.return_code.Value && "SUCCESS" == data.result_code.Value)
                {
                    result.out_trade_no = data.out_trade_no.Value;
                    string trade_state = data.trade_state.Value;
                    result.trade_state = TradeStateEnum.OTHERS;
                    switch(trade_state)
                    {
                        case "SUCCESS":
                            result.trade_state = TradeStateEnum.SUCCESS;
                            break;
                        case "NOTPAY":
                            result.trade_state = TradeStateEnum.NOTPAY;
                            break;
                        case "CLOSED":
                            result.trade_state = TradeStateEnum.CLOSED;
                            break;
                        default:
                            break;
                    }
                    if (result.trade_state == TradeStateEnum.SUCCESS)
                    {
                        result.transaction_id = data.transaction_id.Value;
                        result.total_fee = decimal.Parse(data.total_fee.Value) / 100.00m;
                    }
                }
                else if ("SUCCESS" == data.return_code.Value && "FAIL" == data.result_code.Value)
                {
                    result.trade_state = TradeStateEnum.OTHERS;
                    string err_code = data.err_code.Value;
                    if (err_code == "ORDERNOTEXIST")
                        result.trade_state = TradeStateEnum.NOTEXIST;
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class OrderQueryResult
    {
        public TradeStateEnum trade_state;

        public string out_trade_no;

        public string transaction_id;

        public decimal total_fee;

        public string buyer_logon_id;

        public string buyer_user_id;
    }

    public enum TradeStateEnum
    {
        /// <summary>
        /// 未支付 
        /// </summary>
        NOTPAY=1,

        /// <summary>
        /// 支付成功 
        /// </summary>
        SUCCESS=2,

        // <summary>
        /// 已关闭 
        /// </summary>
        CLOSED=3,

        /// <summary>
        /// 交易不存在
        /// </summary>
        NOTEXIST=4,

        /// <summary>
        /// 其它状态
        /// </summary>
        OTHERS=999

    }
}