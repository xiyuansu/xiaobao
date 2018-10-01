using System;
using System.Collections.Generic;
using System.Web;
using Asiasofti.SmartVehicle.Common;
using System.Collections;
using System.Reflection;
using YR.Web.api.app.privacy;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetLog;
using System.Xml;
using Newtonsoft.Json;

namespace YR.Web.api.baojia
{
    /// <summary>
    /// 宝驾互通接口网关
    /// </summary>
    public class gateway : IHttpHandler
    {
        private static Log Logger = LogFactory.GetLogger(typeof(gateway));

        public void ProcessRequest(HttpContext context)
        {
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "text/plain";

            Hashtable ht = new Hashtable();
            try
            {
                ht = GetRequestParams(context);
                if (ht.Keys.Count > 0)
                {
                    if (ht["action"] == null || ht["action"].ToString().Trim().Length <= 0)
                        throw new Exception("参数不完整");
                    if (ht["sign"] == null || ht["sign"].ToString().Trim().Length <= 0)
                        throw new Exception("参数不完整");

                    Hashtable appht = LoadAclList();
                    string client_id = ht["client_id"].ToString();
                    if (VeryfySign(ht, appht[client_id].ToString()))
                    {
                        string actionName = ht["action"].ToString();
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        object obj = assembly.CreateInstance("YR.Web.api.baojia.service." + actionName);
                        IApiAction action = (IApiAction)obj;

                        //调用相对应的接口方法
                        ApiResp result = action.Execute(ht);
                        context.Response.Write(JsonConvert.SerializeObject(result));

                        Logger.Info("宝驾互通接口调用(成功),访问ip:" + context.Request.UserHostAddress + ",参数:" + JsonConvert.SerializeObject(ht) + ",返回值:" + result);
                    }
                    else
                    {
                        throw new Exception("参数校验失败");
                    }
                }
                else
                {
                    throw new Exception("数据为空");
                }
            }
            catch (Exception e)
            {
                ApiResp resp = new ApiResp();
                resp.code = "-1";
                resp.msg = "系统异常," + e.Message;
                context.Response.Write(JsonConvert.SerializeObject(resp));

                Logger.Error("宝驾互通接口调用(失败),访问ip:" + context.Request.UserHostAddress + ",参数:" + JsonConvert.SerializeObject(ht) + ",异常:" + e.Message);
            }
        }

        /// <summary>
        /// 获取参数列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Hashtable GetRequestParams(HttpContext context)
        {
            Hashtable ht = new Hashtable();
            /*if(context.Request.HttpMethod=="GET")
            {
                foreach(string key in context.Request.QueryString)
                {
                    ht[key] = context.Request.QueryString.Get(key);
                }
            }
            else if (context.Request.HttpMethod == "POST")
            {
                foreach (string key in context.Request.Form)
                {
                    ht[key] = context.Request.Form.Get(key);
                }
            }*/

            foreach (string key in context.Request.QueryString)
            {
                if (!string.IsNullOrEmpty(context.Request.QueryString.Get(key)))
                    ht[key] = context.Request.QueryString.Get(key);
            }
            foreach (string key in context.Request.Form)
            {
                if (!string.IsNullOrEmpty(context.Request.Form.Get(key)))
                    ht[key] = context.Request.Form.Get(key);
            }

            return ht;
        }

        /// <summary>
        /// 对请求参数进行验签
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool VeryfySign(Hashtable ht, string appkey)
        {
            try
            {
                SortedDictionary<string, string> dict = new SortedDictionary<string, string>();

                foreach (string key in ht.Keys)
                {
                    if (key != "sign" && key != "action")
                    {
                        dict.Add(key, ht[key].ToString());
                    }
                }

                string sign_content = "";
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    if (sign_content.Length > 0 && !sign_content.EndsWith("&"))
                        sign_content += "&";
                    sign_content += pair.Key + "=" + System.Web.HttpUtility.UrlDecode(pair.Value);
                }
                sign_content = appkey + "&" + sign_content + "&" + appkey;
                string sign = MD5(sign_content);

                return sign == ht["sign"].ToString();
            }
            catch
            {
                return false;
            }
        }

        private Hashtable LoadAclList()
        {
            Hashtable ht = new Hashtable();

            string xmlfile = HttpContext.Current.Server.MapPath("/api/baojia/acl.xml");
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.Load(xmlfile);

            XmlNodeList nodeList = xml.SelectNodes("/list/app");
            foreach (XmlNode node in nodeList)
            {
                string id = node.Attributes["id"].Value;
                string key = node.Attributes["key"].Value;
                ht.Add(id, key);
            }
            return ht;
        }

        private string MD5(string str)
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(str);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    ret += a.ToString("x2");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}