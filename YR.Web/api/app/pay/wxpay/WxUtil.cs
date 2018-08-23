using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using YR.Common.DotNetCode;

namespace YR.Web.api.app.pay.wxpay
{
    public class WxUtil
    {
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
        /// 获取预支付交易会话标识
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <param name="body"></param>
        /// <param name="paymoney"></param>
        /// <param name="notifyurl"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string GetPrepayId(string out_trade_no,string body, decimal paymoney,string notifyurl,string ip)
        {
            try
            {
                SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                dic.Add("appid", pay.wxpay.WxConfig.AppId);
                dic.Add("body", body);
                dic.Add("mch_id", pay.wxpay.WxConfig.MchId);
                dic.Add("nonce_str", genNonceStr());
                dic.Add("notify_url", notifyurl);
                dic.Add("out_trade_no", out_trade_no);
                dic.Add("spbill_create_ip", ip);
                dic.Add("total_fee", (paymoney * 100).ToString("f0"));
                dic.Add("trade_type", "APP");

                string get_sign = getSign(dic);

                string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
                string _req_data = "<xml>";
                _req_data += "<appid>" + dic["appid"] + "</appid>";
                _req_data += "<body><![CDATA[" + dic["body"] + "]]></body> ";
                _req_data += "<mch_id><![CDATA[" + dic["mch_id"] + "]]></mch_id> ";
                _req_data += "<nonce_str><![CDATA[" + dic["nonce_str"] + "]]></nonce_str> ";
                _req_data += "<notify_url><![CDATA[" + dic["notify_url"] + "]]></notify_url> ";
                _req_data += "<out_trade_no><![CDATA[" + dic["out_trade_no"] + "]]></out_trade_no> ";
                _req_data += "<spbill_create_ip><![CDATA[" + dic["spbill_create_ip"] + "]]></spbill_create_ip> ";
                _req_data += "<total_fee><![CDATA[" + dic["total_fee"] + "]]></total_fee> ";
                _req_data += "<trade_type><![CDATA[" + dic["trade_type"] + "]]></trade_type> ";
                _req_data += "<sign><![CDATA[" + get_sign + "]]></sign> ";
                _req_data += "</xml>";

                string xml = HttpUtil.Post(url, _req_data, "UTF-8");
                dynamic data = new DynamicXml(xml);

                string prepay_id = "";
                if ("SUCCESS" == data.return_code.Value && "SUCCESS" == data.result_code.Value)
                {
                    prepay_id = data.prepay_id.Value;
                }
                return prepay_id;
            }
            catch
            {
                return "";
            }
        }
    }
}