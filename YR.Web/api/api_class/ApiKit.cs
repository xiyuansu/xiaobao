using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YR.Web.api.api_class
{
    public class ApiKit
    {
        private string apiEntry;

        public string ApiEntry
        {
            get { return apiEntry; }
            set { apiEntry = value; }
        }

        private string appKey = SiteHelper.GetAppsetString("BaiduMapServerKey");

        public string AppKey
        {
            get { return appKey; }
        }

        private string appSecret = SiteHelper.GetAppsetString("BaiduMapServerKey");

        public string AppSecret
        {
            get { return appSecret; }
        }

        private static readonly Encoding encoding = Encoding.UTF8;

        private static readonly string DefaultUserAgent = "Api Client v1.0";

        public ApiKit(string apientry)
        {
            this.apiEntry = apientry;
        }

        public String get(Dictionary<String, String> parames)
        {
            String paramStr = buildParamStr(parames);
            HttpWebRequest request = WebRequest.Create(apiEntry + "?" + paramStr) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            HttpWebResponse result = request.GetResponse() as HttpWebResponse;
            StreamReader sr = new StreamReader(result.GetResponseStream(), encoding);
            string strResult = sr.ReadToEnd();
            sr.Close();
            Console.WriteLine(strResult);
            return strResult;
        }

        public String post(Dictionary<String, String> parames)
        {
            String paramStr = buildParamStr(parames);
            HttpWebRequest request;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(paramStr);

            request = WebRequest.Create(apiEntry) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.UserAgent = DefaultUserAgent;
            Stream writeStream = request.GetRequestStream();
            writeStream.Write(bytes, 0, bytes.Length);
            writeStream.Flush();

            HttpWebResponse result = request.GetResponse() as HttpWebResponse;
            StreamReader sr = new StreamReader(result.GetResponseStream(), encoding);
            string strResult = sr.ReadToEnd();
            sr.Close();
            Console.WriteLine(strResult);
            return strResult;
        }

        String buildParamStr(Dictionary<String, String> param)
        {
            String paramStr = String.Empty;
            foreach (var key in param.Keys.ToList())
            {
                if (param.Keys.ToList().IndexOf(key) == 0)
                {
                    paramStr += (key + "=" + param[key]);
                }
                else
                {
                    paramStr += ("&" + key + "=" + param[key]);
                }
            }

            SortedDictionary<string, string> sort_param = new SortedDictionary<string, string>(param);
            string sn = AKSNCaculater.CaculateAKSN(this.appKey, this.appSecret, this.apiEntry, sort_param);
            paramStr += "&sn=" + sn;

            return paramStr;
        }

    }

    class AKSNCaculater
    {
        private static string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        private static string UrlEncode(string str)
        {
            str = System.Web.HttpUtility.UrlEncode(str);
            byte[] buf = Encoding.ASCII.GetBytes(str);//等同于Encoding.ASCII.GetBytes(str)
            for (int i = 0; i < buf.Length; i++)
                if (buf[i] == '%')
                {
                    if (buf[i + 1] >= 'a') buf[i + 1] -= 32;
                    if (buf[i + 2] >= 'a') buf[i + 2] -= 32;
                    i += 2;
                }
            return Encoding.ASCII.GetString(buf);//同上，等同于Encoding.ASCII.GetString(buf)
        }

        private static string HttpBuildQuery(IDictionary<string, string> querystring_arrays)
        {

            StringBuilder sb = new StringBuilder();
            foreach (var item in querystring_arrays)
            {
                sb.Append(UrlEncode(item.Key));
                sb.Append("=");
                sb.Append(UrlEncode(item.Value));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static string CaculateAKSN(string ak, string sk, string url, IDictionary<string, string> querystring_arrays)
        {
            var queryString = HttpBuildQuery(querystring_arrays);

            var str = UrlEncode(url + "?" + queryString + sk);

            return MD5(str);
        }
    }
}
