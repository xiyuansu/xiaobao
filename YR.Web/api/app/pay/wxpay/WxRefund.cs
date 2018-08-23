using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;

namespace YR.Web.api.app.pay.wxpay
{
    public class WxRefund
    {
        private static Log Logger = LogFactory.GetLogger(typeof(WxRefund));

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
        /// 退款操作
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <param name="refund_amount"></param>
        /// <param name="refund_reason"></param>
        /// <returns></returns>
        public bool refund(string out_trade_no, decimal refund_amount, out string errmsg)
        {
            errmsg = "";
            bool result = false;
            try
            {
                SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                dic.Add("appid", pay.wxpay.WxConfig.AppId);
                dic.Add("mch_id", pay.wxpay.WxConfig.MchId);
                dic.Add("nonce_str", genNonceStr());
                dic.Add("out_trade_no", out_trade_no);
                dic.Add("out_refund_no", "t"+out_trade_no);
                dic.Add("total_fee", (refund_amount * 100).ToString("f0"));
                dic.Add("refund_fee", (refund_amount * 100).ToString("f0"));
                dic.Add("op_user_id", pay.wxpay.WxConfig.MchId);

                string get_sign = getSign(dic);

                string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
                string _req_data = "<xml>";
                _req_data += "<appid>" + dic["appid"] + "</appid>";
                _req_data += "<mch_id><![CDATA[" + dic["mch_id"] + "]]></mch_id> ";
                _req_data += "<nonce_str><![CDATA[" + dic["nonce_str"] + "]]></nonce_str> ";
                _req_data += "<out_trade_no><![CDATA[" + dic["out_trade_no"] + "]]></out_trade_no> ";
                _req_data += "<out_refund_no><![CDATA[" + dic["out_refund_no"] + "]]></out_refund_no> ";
                _req_data += "<total_fee><![CDATA[" + dic["total_fee"] + "]]></total_fee> ";
                _req_data += "<refund_fee><![CDATA[" + dic["refund_fee"] + "]]></refund_fee> ";
                _req_data += "<op_user_id><![CDATA[" + dic["op_user_id"] + "]]></op_user_id> ";
                _req_data += "<sign><![CDATA[" + get_sign + "]]></sign> ";
                _req_data += "</xml>";

                string xml = HttpsPost(url, _req_data, "UTF-8");
                dynamic data = new DynamicXml(xml);

                string refund_id = "";
                if ("SUCCESS" == data.return_code.Value && "SUCCESS" == data.result_code.Value)
                {
                    refund_id = data.refund_id.Value;
                    result=true;
                }
                if ("SUCCESS" == data.return_code.Value && "SUCCESS" != data.result_code.Value)
                {
                    errmsg = data.err_code_des.Value;
                }
                Logger.Warn(string.Format("微信押金退款{0}({1}):{2}", result ? "成功" : "失败", out_trade_no, errmsg));
                return result;
            }
            catch (Exception ex)
            {
                Logger.Warn(string.Format("微信押金退款失败({0}):{1}", out_trade_no, ex.Message));
                return false;
            }
        }

        public bool refund2(string transationid, decimal refund_amount,out string errmsg)
        {
            errmsg = "";
            bool result = false;
            try
            {
                SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                dic.Add("appid", pay.wxpay.WxConfig.AppId);
                dic.Add("mch_id", pay.wxpay.WxConfig.MchId);
                dic.Add("nonce_str", genNonceStr());
                dic.Add("transaction_id", transationid);
                dic.Add("out_refund_no", "t" + transationid);
                dic.Add("total_fee", (refund_amount * 100).ToString("f0"));
                dic.Add("refund_fee", (refund_amount * 100).ToString("f0"));
                dic.Add("op_user_id", pay.wxpay.WxConfig.MchId);

                string get_sign = getSign(dic);

                string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
                string _req_data = "<xml>";
                _req_data += "<appid>" + dic["appid"] + "</appid>";
                _req_data += "<mch_id><![CDATA[" + dic["mch_id"] + "]]></mch_id> ";
                _req_data += "<nonce_str><![CDATA[" + dic["nonce_str"] + "]]></nonce_str> ";
                _req_data += "<transaction_id><![CDATA[" + dic["transaction_id"] + "]]></transaction_id> ";
                _req_data += "<out_refund_no><![CDATA[" + dic["out_refund_no"] + "]]></out_refund_no> ";
                _req_data += "<total_fee><![CDATA[" + dic["total_fee"] + "]]></total_fee> ";
                _req_data += "<refund_fee><![CDATA[" + dic["refund_fee"] + "]]></refund_fee> ";
                _req_data += "<op_user_id><![CDATA[" + dic["op_user_id"] + "]]></op_user_id> ";
                _req_data += "<sign><![CDATA[" + get_sign + "]]></sign> ";
                _req_data += "</xml>";

                string xml = HttpsPost(url, _req_data, "UTF-8");
                dynamic data = new DynamicXml(xml);

                string refund_id = "";
                if ("SUCCESS" == data.return_code.Value && "SUCCESS" == data.result_code.Value)
                {
                    refund_id = data.refund_id.Value;
                    result = true;

                }
                if ("SUCCESS" == data.return_code.Value && "SUCCESS" != data.result_code.Value)
                {
                    errmsg = data.err_code_des.Value;
                }
                Logger.Warn(string.Format("微信押金退款{0}({1}):{2}", result ? "成功" : "失败", transationid, errmsg));
                return result;
            }
            catch (Exception ex)
            {
                Logger.Warn(string.Format("微信押金退款失败({0}):{1}", transationid, ex.Message));
                return false;
            }
        }

        private string HttpsPost(string url, string postDataStr, string codename)
        {
            Encoding encoding = Encoding.GetEncoding(codename);

            string cert ="", password = WxConfig.MchId;
            cert=SiteHelper.GetAppsetString("wx_api_cert");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
            {
                if (errors == SslPolicyErrors.None)
                    return true;
                return false;
            };
            X509Certificate cer = new X509Certificate(cert, password);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ClientCertificates.Add(cer);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            // 增加下面两个属性即可
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;

            byte[] bs = encoding.GetBytes(postDataStr);
            request.ContentLength = bs.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(bs, 0, bs.Length);
            myRequestStream.Flush();
            myRequestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            Logger.Warn(string.Format("https://api.mch.weixin.qq.com/secapi/pay/refund 返回结果({0})", retString));
            return retString;
        }
    }
}