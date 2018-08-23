using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Data;
using System.Collections;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using YR.Common.DotNetConfig;
using YR.Common.DotNetJson;

namespace Asiasofti.SmartVehicle.Common
{
    /// <summary>
    /// 网站辅助类
    /// </summary>
    public static class SiteHelper
    {
        /// <summary>
        /// 得到WebConfig AppSetting的值
        /// </summary>
        /// <param name="key">AppSetting key</param>
        /// <returns>AppSetting key<</returns>
        public static string GetAppsetString(string key)
        {
            return ConfigHelper.GetAppSettings(key);
        }

        public static string GetAppsetString(string key,string host)
        {
            return ConfigHelper.GetAppSettings(key,host);
        }

        /// <summary>
        /// 得到Web服务器的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWebServerCurrentTime()
        {
            return DateTime.Now;
        }
        /// <summary>
        /// 得到数据库服务器的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDBServerCurrentTime()
        {
            return DateTime.Parse("");
        }
        /// <summary>
        /// GET方式请求指定url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String GetRequest(String url)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                hwRequest = (System.Net.HttpWebRequest)WebRequest.Create(url);
                //hwRequest.Timeout = 5000;
                hwRequest.Method = "GET";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
            }
            catch (System.Exception err)
            {
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch (System.Exception err)
            {
            }

            return strResult;
        }
        /// <summary>
        /// POST方式请求指定url
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="bData">post参数</param>
        /// <returns></returns>
        public static String PostRequest(string url, byte[] bData)
        {
            System.Net.HttpWebRequest hwRequest;
            System.Net.HttpWebResponse hwResponse;
            string strResult = string.Empty;
            try
            {
                hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                //hwRequest.Timeout = 5000;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwRequest.ContentLength = bData.Length;
                System.IO.Stream smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch (System.Exception err)
            {
                //WriteErrLog(err.ToString());
                return strResult;
            }
            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch (System.Exception err)
            {
                //WriteErrLog(err.ToString());
            }
            return strResult;
        }
        /// <summary>
        /// MD5加密方法
        /// </summary>
        /// <param name="toCryString"></param>
        /// <returns></returns>
        public static string MD5(string toCryString)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(toCryString, "MD5");
        }
        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <param name="numberFlag"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandom(bool numberFlag, int length)
        {
            string retStr = "";
            string strTable = numberFlag ? "1234567890" : "1234567890abcdefghijkmnpqrstuvwxyz";
            int len = strTable.Length;
            bool bDone = true;
            do
            {
                retStr = "";
                int count = 0;
                for (int i = 0; i < length; i++)
                {
                    double dblR = new Random((int)DateTime.Now.Ticks).Next(0, len);
                    int intR = (int)Math.Floor(dblR);
                    char c = strTable.ToCharArray()[intR];
                    if (('0' <= c) && (c <= '9'))
                    {
                        count++;
                    }
                    retStr += strTable.ToCharArray()[intR];
                    System.Threading.Thread.Sleep(10);
                }
                if (count >= 2)
                {
                    bDone = false;
                }
            } while (bDone);

            return retStr;
        }

        /// <summary>
        /// 256位AES加密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <returns>如果失败返回空字符串</returns>
        public static string Encrypt(string toEncrypt, string key)
        {
            try
            {
                // 256-AES key    
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception e)
            {
                //如果失败返回空字符串
                return "";
            }
        }

        /// <summary>
        /// 256位AES解密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <returns>如果失败返回空字符串</returns>
        public static string Decrypt(string toDecrypt, string key)
        {
            try
            {
                // 256-AES key    
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception e)
            {
                //如果失败返回空字符串
                return "";
            }
        }

        /// <summary>
        /// 将查询字符串转化成hashtable
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Hashtable GetHashTableByQueryString(string query)
        {
            Hashtable res = new Hashtable();
            string[] querys = query.Split(new char[] { '&' });
            foreach (string str in querys)
            {
                int num = str.IndexOf("=");
                string key = str.Substring(0, num);
                string value = str.Substring(num + 1, str.Length - num - 1);
                //string[] kv = str.Split(new char[] { '=' });
                res[key] = value;
            }
            return res;
        }

        /// <summary>
        /// 根据hashtable得到json字符串
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetJsonFromHashTable(Hashtable htmessage, string state, string message)
        {
            StringBuilder jsonStr = new StringBuilder();
            jsonStr.Append("{\"state\": \"" + state + "\",\"message\":\"" + message + "\",\"content\":{");
            if (htmessage != null)
            {
                if (htmessage.Keys.Count > 0)
                {
                    var listkv = htmessage.GetEnumerator();
                    StringBuilder keys = new StringBuilder();
                    while (listkv.MoveNext())
                    {
                        keys.Append(listkv.Key.ToString()).Append(",");
                    }
                    string[] kvs = keys.ToString().Split(new char[] { ',' });

                    for (int i = 0; i < kvs.Length; i++)
                    {
                        if (kvs[i].Length > 0)
                        {
                            if (i == 0)
                            {
                                jsonStr.Append("\"").Append(kvs[i]).Append("\"").Append(":").Append("\"").Append(htmessage[kvs[i]].ToString().Trim()).Append("\"");
                            }
                            else
                            {
                                jsonStr.Append(",\"").Append(kvs[i]).Append("\"").Append(":").Append("\"").Append(htmessage[kvs[i]].ToString().Trim()).Append("\"");
                            }
                        }
                    }
                }
            }
            jsonStr.Append("}}");
            return jsonStr.ToString();
        }

        public static string GetJsonFromHashTable(Hashtable htmessage, string state, string message, string entityName)
        {
            StringBuilder jsonStr = new StringBuilder();
            jsonStr.Append("{\"state\": \"" + state + "\",\"message\":\"" + message + "\",\"content\":{");
            if (entityName.Trim().Length > 0)
            {
                jsonStr.Append("\"").Append(entityName).Append("\":{");
            }
            if (htmessage != null)
            {
                if (htmessage.Keys.Count > 0)
                {
                    var listkv = htmessage.GetEnumerator();
                    StringBuilder keys = new StringBuilder();
                    while (listkv.MoveNext())
                    {
                        keys.Append(listkv.Key.ToString()).Append(",");
                    }
                    string[] kvs = keys.ToString().Split(new char[] { ',' });

                    for (int i = 0; i < kvs.Length; i++)
                    {
                        if (kvs[i].Length > 0)
                        {
                            if (i == 0)
                            {
                                jsonStr.Append("\"").Append(kvs[i]).Append("\"").Append(":").Append("\"").Append(htmessage[kvs[i]].ToString().Trim()).Append("\"");
                            }
                            else
                            {
                                jsonStr.Append(",\"").Append(kvs[i]).Append("\"").Append(":").Append("\"").Append(htmessage[kvs[i]].ToString().Trim()).Append("\"");
                            }
                        }
                    }
                }
            }
            jsonStr.Append("}}}");
            return jsonStr.ToString();
        }

        public static string GetJsonFromHashTable2(Hashtable htmessage, string state, string message, string entityName)
        {
            StringBuilder jsonStr = new StringBuilder();
            jsonStr.Append("{\"state\": \"" + state + "\",\"message\":\"" + message + "\",\"content\":{");
            if (entityName.Trim().Length > 0)
            {
                jsonStr.Append("\"").Append(entityName).Append("\":");
            }
            if (htmessage != null)
            {
                jsonStr.Append(Newtonsoft.Json.JsonConvert.SerializeObject(htmessage));
            }
            else
            {
                jsonStr.Append("null");
            }
            jsonStr.Append("}}");
            return jsonStr.ToString();
        }

        public static string GetJsonFromDataTable2(DataTable dt, string state, string message, string entityName, int count)
        {
            StringBuilder jsonStr = new StringBuilder();
            jsonStr.Append("{\"state\": \"" + state + "\",\"message\":\"" + message + "\",\"content\":{");
            jsonStr.Append("\"count\":\"" + count + "\",");
            if (entityName.Trim().Length > 0)
            {
                jsonStr.Append("\"").Append(entityName).Append("\":");
            }
            if (dt != null)
            {
                jsonStr.Append(Newtonsoft.Json.JsonConvert.SerializeObject(dt));
            }
            else
            {
                jsonStr.Append("null");
            }
            jsonStr.Append("}}");
            return jsonStr.ToString();
        }
        
        /// <summary>
        /// 获取当前服务器地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebRoot()
        {
            string url = SiteHelper.GetAppsetString("webRoot");
            return url.ToString();
        }

        /// <summary>
        /// 获取当前的文件服务器地址
        /// </summary>
        /// <returns></returns>
        public static string GetFileServer()
        {
            string url = SiteHelper.GetAppsetString("FileServer");
            return url.ToString();
        }

        /// <summary>
        /// 根据hashtable key获取hashtalbe value
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetHashTableValueByKey(Hashtable ht, string key)
        {
            string res = string.Empty;
            if (ht[key] != null)
            {
                res = ht[key].ToString();
            }
            else if (ht[key.ToUpper()] != null)
            {
                res = ht[key.ToUpper()].ToString();
            }
            else if (ht[key.ToLower()] != null)
            {
                res = ht[key.ToLower()].ToString();
            }
            else
            {
                res = "";
            }
            return res.Trim();
        }
        /// <summary>
        /// 根据经纬度获取位置信息
        /// </summary>
        /// <param name="weidu"></param>
        /// <param name="jingdu"></param>
        /// <param name="coordtype">坐标类型：bd09ll，bd09mc，gcj02ll，wgs84ll</param>
        /// <returns></returns>
        public static string GetLocationByGPS(string weidu, string jingdu, string coordtype = "bd09ll")
        {
            string location = string.Empty;
            string url = GetAppsetString("LocationURL");
            string ak = GetAppsetString("BaiduMapServerKey");
            StringBuilder reqUrl = new StringBuilder();
            reqUrl.Append("ak=").Append(ak).Append("&callback=&output=json&pois=1&coordtype=").Append(coordtype);
            reqUrl.Append("&location=").Append(weidu).Append(",").Append(jingdu);
            string baiduUrl = url + "?" + reqUrl.ToString();
            byte[] bTemp = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(reqUrl.ToString());
            string res = PostRequest(url, bTemp);
            try
            {
                var js = new JavaScriptSerializer();
                var info = js.Deserialize<Dictionary<string, object>>(res);
                var result = info["result"] as Dictionary<string, object>;
                foreach (var kvp in result)
                {
                    if (kvp.Key == "formatted_address")
                    {
                        location = kvp.Value.ToString();
                        break;
                    }
                    //if (kvp.Key == "pois")
                    //{
                    //    var vss = kvp.Value as ArrayList;
                    //    var dvs = vss[1] as Dictionary<string, object>;
                    //    location = dvs["addr"].ToString();
                    //    break;
                    //}
                }
            }
            catch
            {

            }
            return location;
        }
        /// <summary>
        /// 根据gps经纬度转换成百度经纬度
        /// </summary>
        /// <param name="weidu"></param>
        /// <param name="jingdu"></param>
        /// <returns></returns>
        public static LatLng GetBaiduByGPS(string weidu, string jingdu)
        {
            LatLng latlng = null;
            string url = GetAppsetString("BaiduGeoConvertURL");
            string ak = GetAppsetString("BaiduMapServerKey");
            StringBuilder reqUrl = new StringBuilder();
            reqUrl.Append(url).Append("?ak=").Append(ak);
            reqUrl.Append("&from=1&to=5&output=json");
            reqUrl.Append("&coords=").Append(jingdu).Append(",").Append(weidu);
            string baiduUrl = reqUrl.ToString();
            string res = GetRequest(baiduUrl);
            try
            {
                JObject o = JObject.Parse(res);
                string status = o["status"].ToString();
                if (status.CompareTo("0") == 0)
                {
                    double longitude = double.Parse(o["result"][0]["x"].ToString());
                    double latitude = double.Parse(o["result"][0]["y"].ToString());
                    latlng = new LatLng(latitude, longitude);
                }
            }
            catch
            {

            }
            return latlng;
        }

        /// <summary>
        /// 根据gps经纬度转换成gcj02经纬度
        /// </summary>
        /// <param name="weidu"></param>
        /// <param name="jingdu"></param>
        /// <returns></returns>
        public static LatLng GPSToGCJ02(string longitude, string latitude)
        {
            LatLng latlng = null;
            string api_url = "http://restapi.amap.com/v3/assistant/coordinate/convert";
            string key = GetAppsetString("AMapServerKey");
            string requestData = string.Format("key={0}&locations={1},{2}&coordsys=gps&output=JSON",key,longitude,latitude);
            string res = GetRequest(api_url+"?"+requestData);
            try
            {
                JObject o = JObject.Parse(res);
                string status = o["status"].ToString();
                if (status.CompareTo("1") == 0)
                {
                    double lng = double.Parse(o["locations"].ToString().Split(new char[]{','})[0]);
                    double lat = double.Parse(o["locations"].ToString().Split(new char[] { ',' })[1]);
                    latlng = new LatLng(lat, lng);
                }
            }
            catch
            {
                latlng = null;
            }
            return latlng;
        }

        /// <summary>
        /// 根据百度地图获取建议的地址
        /// </summary>
        /// <param name="add"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static string GetAddressSuggestion(string add, string region, string location)
        {
            string url = GetAppsetString("SuggestionURL");
            string ak = GetAppsetString("BaiduMapServerKey");
            StringBuilder reqUrl = new StringBuilder();
            //reqUrl.Append("region=").Append(region).Append("&output=json&ak=pZsoEIbsY33xR1akjzweD9mV").Append("&query=").Append(add).Append("&location=").Append(location);
            reqUrl.Append("region=").Append(region).Append("&output=json&ak=").Append(ak).Append("&query=").Append(add);
            // byte[] bTemp = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(reqUrl.ToString());
            // string res = PostRequest(url, bTemp);
            string res = GetRequest(url + "?" + reqUrl.ToString());
            return res;
        }
        /// <summary>  
        /// 生成订单号  
        /// </summary>  
        /// <returns></returns>  
        public static string GenerateOrderNum()
        {
            StringBuilder sbNumber = new StringBuilder();
            DateTime dtnow = GetWebServerCurrentTime();
            sbNumber.Append(dtnow.Year.ToString());//4  
            sbNumber.Append(dtnow.Month.ToString().Length == 1 ? "0" + dtnow.Month.ToString() : dtnow.Month.ToString());//2  
            sbNumber.Append(dtnow.Day.ToString().Length == 1 ? "0" + dtnow.Day.ToString() : dtnow.Day.ToString());//2  
            sbNumber.Append(dtnow.ToString().Length == 1 ? "0" + dtnow.Hour.ToString() : dtnow.Hour.ToString());//2  
            sbNumber.Append(dtnow.Minute.ToString().Length == 1 ? "0" + dtnow.Minute.ToString() : dtnow.Minute.ToString());//2  
            sbNumber.Append(dtnow.Second.ToString().Length == 1 ? "0" + dtnow.Second.ToString() : dtnow.Second.ToString());//2  
            long loid = long.Parse(sbNumber.ToString()) * 3;
            return loid.ToString() + dtnow.Millisecond.ToString();

        }

        /// <summary>  
        /// 生成支付单号  
        /// </summary>  
        /// <returns></returns>  
        public static string GeneratePayID()
        {
            string id = "";
            id = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"),CreateRandom(true,5));
            return id;
        }

        public static Dictionary<string, object> ConvertDictionary(string res)
        {
            var js = new JavaScriptSerializer();
            return js.Deserialize<Dictionary<string, object>>(res);
        }
        /// <summary>
        /// 调用Webpage
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string CallWebPageFromGet(string strUrl, WebHeaderCollection header = null)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(strUrl);
                httpRequest.Timeout = 2000;
                httpRequest.Method = "GET";
                //httpRequest.ContentType = "application/xml";
                //httpRequest.Accept = "application/json";
                httpRequest.Headers = header;
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
                string result = sr.ReadToEnd();
                result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                sr.Close();
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

            //System.Net.HttpWebRequest webrequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);

            //HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();//请求连接,并返回数据
            //Stream stream = webresponse.GetResponseStream();//把返回数据转换成流文件
            //byte[] rsByte = new Byte[webresponse.ContentLength];  //把流文件转换为字节数组

            //try
            //{
            //    stream.Read(rsByte, 0, (int)webresponse.ContentLength);
            //    return rsByte;
            //}
            //catch (Exception exp)
            //{
            //    exp.ToString();
            //    return new byte[] { };
            //}
        }

        public static string CallWebPageFromPost(string Url, string postDataStr, WebHeaderCollection header=null)
        {
            string rs = null;

            System.GC.Collect();
            CookieContainer cookieContainer = new CookieContainer();
            // 设置提交的相关参数 
            HttpWebRequest request = null;
            HttpWebResponse SendSMSResponse = null;
            Stream dataStream = null;
            StreamReader SendSMSResponseStream = null;
            try
            {
                request = WebRequest.Create(Url) as HttpWebRequest;
                request.Method = "POST";
                //request.KeepAlive = false;
                //request.ServicePoint.ConnectionLimit = 300;
                //request.AllowAutoRedirect = true;
                request.Timeout = 10000;
                request.ReadWriteTimeout = 10000;
                request.ContentType = "application/json";
                request.Accept = "application/json";
                if (header != null)
                {
                    request.Headers = header;
                }
                //request.Headers.Add("X-Auth-Token", HttpUtility.UrlEncode("openstack"));
                byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
                //request.Proxy = null;
                //request.CookieContainer = cookieContainer;
                using (dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bytes, 0, bytes.Length);
                }
                SendSMSResponse = (HttpWebResponse)request.GetResponse();
                if (SendSMSResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    if (SendSMSResponse != null)
                    {
                        SendSMSResponse.Close();
                        SendSMSResponse = null;
                    }
                    if (request != null)
                    {
                        request.Abort();
                    }
                    return null;
                }
                SendSMSResponseStream = new StreamReader(SendSMSResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                rs = SendSMSResponseStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (dataStream != null)
                {
                    dataStream.Close();
                    dataStream.Dispose();
                    dataStream = null;
                }
                if (SendSMSResponseStream != null)
                {
                    SendSMSResponseStream.Close();
                    SendSMSResponseStream.Dispose();
                    SendSMSResponseStream = null;
                }
                if (SendSMSResponse != null)
                {
                    SendSMSResponse.Close();
                    SendSMSResponse = null;
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            finally
            {
                if (dataStream != null)
                {
                    dataStream.Close();
                    dataStream.Dispose();
                    dataStream = null;
                }
                if (SendSMSResponseStream != null)
                {
                    SendSMSResponseStream.Close();
                    SendSMSResponseStream.Dispose();
                    SendSMSResponseStream = null;
                }
                if (SendSMSResponse != null)
                {
                    SendSMSResponse.Close();
                    SendSMSResponse = null;
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return rs;
        }
        /// <summary>
        /// 时间差
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(分钟)单位</returns>
        public static int ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return (int)Math.Ceiling(ts3.TotalMinutes);
        }

        /// <summary>
        /// 时间差
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(秒)单位</returns>
        public static int ExecDateDiffSecond(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return (int)Math.Ceiling(ts3.TotalSeconds);
        }
        /// <summary>
        /// 转换分钟
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static decimal ConventMinutes(string seconds)
        {
            decimal minutes = 1;
            decimal.TryParse(seconds, out minutes);
            if (minutes % 60 == 0)
            {
                minutes
                    = minutes / 60;
            }
            else
            {
                minutes = (minutes - minutes % 60) / 60 + 1;
            }
            if (minutes == 0)
            {
                minutes = minutes + 1;
            }
            return Math.Ceiling(minutes);
        }
        /// <summary>
        /// 转换公里
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static decimal ConvertMileage(string mi)
        {
            decimal mileage = 0;
            decimal.TryParse(mi, out mileage);
            if (mileage % 1000 == 0)
            {
                mileage
                    = mileage / 1000;
            }
            else
            {
                mileage = (mileage - mileage % 1000) / 1000 + 1;
            }
            return Math.Ceiling(mileage);
        }
        /// <summary>
        /// gps经纬度转换成高德地图经纬度
        /// </summary>
        /// <param name="wgLoc"></param>
        /// <returns></returns>
        public static LatLng TransformFromWGSToGCJ(LatLng wgLoc)
        {

            double a = 6378245.0;
            double ee = 0.00669342162296594323;
            //如果在国外，则默认不进行转换
            if (outOfChina(wgLoc.latitude, wgLoc.longitude))
            {
                return new LatLng(wgLoc.latitude, wgLoc.longitude);
            }
            double dLat = transformLat(wgLoc.longitude - 105.0, wgLoc.latitude - 35.0);
            double dLon = transformLon(wgLoc.longitude - 105.0, wgLoc.latitude - 35.0);
            double radLat = wgLoc.latitude / 180.0 * Math.PI;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * Math.PI);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * Math.PI);

            return new LatLng(wgLoc.latitude + dLat, wgLoc.longitude + dLon);
        }

        /** 
        * GPS坐标转换为百度坐标 
        * @param gg_lat 
        * @param gg_lon 
        */
        public static LatLng TransformFromWGSToBD(double gg_lat, double gg_lon)
        {
            LatLng latlng = new LatLng(gg_lat, gg_lon);
            latlng=TransformFromWGSToGCJ(latlng);
            double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
            double x = latlng.longitude, y = latlng.latitude;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
            latlng.longitude = z * Math.Cos(theta) + 0.0065;
            latlng.latitude = z * Math.Sin(theta) + 0.006;
            return latlng;
        }

        public static double transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(x > 0 ? x : -x);
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * Math.PI) + 40.0 * Math.Sin(y / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * Math.PI) + 320 * Math.Sin(y * Math.PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }
        public static double transformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(x > 0 ? x : -x);
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * Math.PI) + 40.0 * Math.Sin(x / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * Math.PI) + 300.0 * Math.Sin(x / 30.0 * Math.PI)) * 2.0 / 3.0;
            return ret;
        }
        public static bool outOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 计算坐标相差距离，单位：千米
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double EARTH_RADIUS = 6378.137;//地球半径
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 判断点是否在多边形内
        /// </summary>
        /// <param name="point">检测点</param>
        /// <param name="pts">多边形的顶点</param>
        /// <returns>点在多边形内返回true,否则返回false</returns>
        public static bool IsPtInPoly(LatLng point, List<LatLng> pts)
        {
            if (pts.Count < 3)
                return false;

            int N = pts.Count;
            bool boundOrVertex = true; //如果点位于多边形的顶点或边上，也算做点在多边形内，直接返回true
            int intersectCount = 0;//cross points count of x 
            double precision = 2e-10; //浮点类型计算时候与0比较时候的容差
            LatLng p1, p2;//neighbour bound vertices
            LatLng p = point; //当前点

            p1 = pts[0];//left vertex        
            for (int i = 1; i <= N; ++i)
            {//check all rays            
                if (p.longitude==p1.longitude && p.latitude==p1.latitude)
                {
                    return boundOrVertex;//p is an vertex
                }

                p2 = pts[i % N];//right vertex            
                if (p.longitude < Math.Min(p1.longitude, p2.longitude) || p.longitude > Math.Max(p1.longitude, p2.longitude))
                {//ray is outside of our interests                
                    p1 = p2;
                    continue;//next ray left point
                }

                if (p.longitude > Math.Min(p1.longitude, p2.longitude) && p.longitude < Math.Max(p1.longitude, p2.longitude))
                {//ray is crossing over by the algorithm (common part of)
                    if (p.latitude <= Math.Max(p1.latitude, p2.latitude))
                    {//x is before of ray                    
                        if (p1.longitude == p2.longitude && p.latitude >= Math.Min(p1.latitude, p2.latitude))
                        {//overlies on a horizontal ray
                            return boundOrVertex;
                        }

                        if (p1.latitude == p2.latitude)
                        {//ray is vertical                        
                            if (p1.latitude == p.latitude)
                            {//overlies on a vertical ray
                                return boundOrVertex;
                            }
                            else
                            {//before ray
                                ++intersectCount;
                            }
                        }
                        else
                        {//cross point on the left side                        
                            double xinters = (p.longitude - p1.longitude) * (p2.latitude - p1.latitude) / (p2.longitude - p1.longitude) + p1.latitude;//cross point of y                        
                            if (Math.Abs(p.latitude - xinters) < precision)
                            {//overlies on a ray
                                return boundOrVertex;
                            }

                            if (p.latitude < xinters)
                            {//before ray
                                ++intersectCount;
                            }
                        }
                    }
                }
                else
                {//special case when ray is crossing through the vertex                
                    if (p.longitude == p2.longitude && p.latitude <= p2.latitude)
                    {//p crossing over p2                    
                        LatLng p3 = pts[(i + 1) % N]; //next vertex                    
                        if (p.longitude >= Math.Min(p1.longitude, p3.longitude) && p.longitude <= Math.Max(p1.longitude, p3.longitude))
                        {//p.x lies between p1.x & p3.x
                            ++intersectCount;
                        }
                        else
                        {
                            intersectCount += 2;
                        }
                    }
                }
                p1 = p2;//next ray left point
            }

            if (intersectCount % 2 == 0)
            {//偶数在多边形外
                return false;
            }
            else
            { //奇数在多边形内
                return true;
            }
        }

        /// <summary>  
        /// 判断点是否在多边形内.  
        /// ----------原理----------  
        /// 注意到如果从P作水平向左的射线的话，如果P在多边形内部，那么这条射线与多边形的交点必为奇数，  
        /// 如果P在多边形外部，则交点个数必为偶数(0也在内)。  
        /// </summary>  
        /// <param name="checkPoint">要判断的点</param>  
        /// <param name="polygonPoints">多边形的顶点</param>  
        /// <returns></returns>  
        public static bool IsInPolygon(LatLng checkPoint, List<LatLng> polygonPoints)
        {
            bool inside = false;
            int pointCount = polygonPoints.Count;
            LatLng p1, p2;
            for (int i = 0, j = pointCount - 1; i < pointCount; j = i, i++)//第一个点和最后一个点作为第一条线，之后是第一个点和第二个点作为第二条线，之后是第二个点与第三个点，第三个点与第四个点...  
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                if (checkPoint.latitude < p2.latitude)
                {//p2在射线之上  
                    if (p1.latitude <= checkPoint.latitude)
                    {//p1正好在射线中或者射线下方  
                        if ((checkPoint.latitude - p1.latitude) * (p2.longitude - p1.longitude) > (checkPoint.longitude - p1.longitude) * (p2.latitude - p1.latitude))//斜率判断,在P1和P2之间且在P1P2右侧  
                        {
                            //射线与多边形交点为奇数时则在多边形之内，若为偶数个交点时则在多边形之外。  
                            //由于inside初始值为false，即交点数为零。所以当有第一个交点时，则必为奇数，则在内部，此时为inside=(!inside)  
                            //所以当有第二个交点时，则必为偶数，则在外部，此时为inside=(!inside)  
                            inside = (!inside);
                        }
                    }
                }
                else if (checkPoint.latitude < p1.latitude)
                {
                    //p2正好在射线中或者在射线下方，p1在射线上  
                    if ((checkPoint.latitude - p1.latitude) * (p2.longitude - p1.longitude) < (checkPoint.longitude - p1.longitude) * (p2.latitude - p1.latitude))//斜率判断,在P1和P2之间且在P1P2右侧  
                    {
                        inside = (!inside);
                    }
                }
            }
            return inside;
        }

        /// <summary>
        /// 根据坐标(wgs84)获取地址信息
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="citycode">城市代码</param>
        /// <param name="cityname">城市名称</param>
        /// <returns></returns>
        public static string GetAddressInfoByGPS(string longitude, string latitude,out string citycode,out string cityname)
        {
            string address = "";
            citycode = "";
            cityname = "";
            try
            {
                LatLng latlng = new LatLng(double.Parse(latitude), double.Parse(longitude));
                latlng = TransformFromWGSToGCJ(latlng);
                string location = string.Empty;
                string url = "http://restapi.amap.com/v3/geocode/regeo";
                string key = GetAppsetString("AMapServerKey");
                StringBuilder reqParameters = new StringBuilder();
                reqParameters.Append("key=").Append(key);
                reqParameters.Append("&location=").Append(longitude + "," + latitude);
                string apiUrl = url + "?" + reqParameters.ToString();
                string res = GetRequest(apiUrl);
                var js = new JavaScriptSerializer();
                var info = js.Deserialize<Dictionary<string, object>>(res);
                var regeocode = info["regeocode"] as Dictionary<string, object>;
                var addressComponent = regeocode["addressComponent"] as Dictionary<string, object>;
                if (addressComponent != null)
                {
                    citycode = addressComponent["citycode"].ToString();
                    cityname = addressComponent["city"].ToString();
                    if(addressComponent["city"] is ArrayList || string.IsNullOrEmpty(cityname))
                        cityname = addressComponent["province"].ToString();
                    address = regeocode["formatted_address"].ToString();
                }
                return address;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据坐标(wgs84)获取地址信息
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="citycode">城市代码</param>
        /// <param name="cityname">城市名称</param>
        /// <param name="districtcode">区代码</param>
        /// <param name="districtname">区名称</param>
        /// <returns></returns>
        public static string GetAddressInfoByGPS(string longitude, string latitude, out string citycode, out string cityname,out string districtcode,out string districtname)
        {
            string address = "";
            citycode = "";
            cityname = "";
            districtname = "";
            districtcode = "";
            try
            {
                LatLng latlng = new LatLng(double.Parse(latitude), double.Parse(longitude));
                latlng = TransformFromWGSToGCJ(latlng);
                string location = string.Empty;
                string url = "http://restapi.amap.com/v3/geocode/regeo";
                string key = GetAppsetString("AMapServerKey");
                StringBuilder reqParameters = new StringBuilder();
                reqParameters.Append("key=").Append(key);
                reqParameters.Append("&location=").Append(longitude + "," + latitude);
                string apiUrl = url + "?" + reqParameters.ToString();
                string res = GetRequest(apiUrl);
                var js = new JavaScriptSerializer();
                var info = js.Deserialize<Dictionary<string, object>>(res);
                var regeocode = info["regeocode"] as Dictionary<string, object>;
                var addressComponent = regeocode["addressComponent"] as Dictionary<string, object>;
                if (addressComponent != null)
                {
                    citycode = addressComponent["citycode"].ToString();
                    cityname = addressComponent["city"].ToString();
                    if (addressComponent["city"] is ArrayList || string.IsNullOrEmpty(cityname))
                        cityname = addressComponent["province"].ToString();
                    districtname = addressComponent["district"].ToString();
                    districtcode = addressComponent["adcode"].ToString();
                    address = regeocode["formatted_address"].ToString();
                }
                return address;
            }
            catch
            {
                return null;
            }
        }
    }

    public class LatLng
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public LatLng(double _latitude, double _longitude)
        {
            latitude = _latitude;
            longitude = _longitude;
        }
    }
}
