using System;
using System.Collections.Generic;
using System.Web;
using Asiasofti.SmartVehicle.Common;
using System.Collections;
using System.Reflection;
using YR.Web.api.app.privacy;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using System.Xml;

namespace YR.Web.api.weapp
{
    /// <summary>
    /// 运维接口网关
    /// </summary>
    public class RequestControler : IHttpHandler
    {
        private static Log Logger = LogFactory.GetLogger(typeof(RequestControler));

        public void ProcessRequest(HttpContext context)
        {
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "text/plain";

            string requestData = "";
            try
            {
                if(!CheckAuth(context))
                {
                    throw new Exception("无接口访问权限,请联系管理员");
                }

                string clientData = "";
                Hashtable ht = new Hashtable();
                foreach (string key in context.Request.Form.AllKeys)
                {
                    ht.Add(key, context.Request.Form.Get(key));

                    if (clientData.Length>0 && !clientData.EndsWith("&"))
                        clientData += "&";
                    clientData += key + "=" + context.Request.Form.Get(key);
                }

                if (clientData != null && clientData.Length > 0)
                {
                    requestData = clientData;
                    if (requestData.Length > 0)
                    {
                        if (ht["type"] == null || ht["type"].ToString().Trim().Length <= 0)
                            throw new Exception("参数不完整");
                        if ((ht["UID"] == null || ht["UID"].ToString().Trim().Length <= 0))
                            throw new Exception("参数不完整");

                        string typeName = ht["type"].ToString();
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        object obj = assembly.CreateInstance("YR.Web.api.weapp.privacy." + typeName);
                        if(obj==null)
                            obj = assembly.CreateInstance("YR.Web.api.app.privacy." + typeName);
                        IApiAction2 action = (IApiAction2)obj;

                        //调用相对应的接口方法
                        string result = action.Execute(ht);

                        context.Response.Write(result);

                        Logger.Info("微信小程序API接口调用(成功),访问ip:" + context.Request.UserHostAddress + ",参数:" + requestData + ",返回值:" + result);
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
                string result = SiteHelper.GetJsonFromHashTable(null, "faild", "系统异常," + e.Message);
                context.Response.Write(result);

                Logger.Error("微信小程序API接口调用(失败),访问ip:" + context.Request.UserHostAddress + ",参数:" + requestData + ",异常:" + e.Message);
            }
        }

        /// <summary>
        /// 检查接口调用授权
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool CheckAuth(HttpContext context)
        {
            bool result = false;
            string appid = context.Request["appid"];
            string appkey = context.Request["appkey"];
            Hashtable appht = LoadAclList();
            if (appht.ContainsKey(appid))
            {
                if (appht[appid].ToString() == appkey)
                    result = true;
            }

            return result;
        }

        private Hashtable LoadAclList()
        {
            Hashtable ht = new Hashtable();

            string xmlfile = HttpContext.Current.Server.MapPath("/api/weapp/acl.xml");
            XmlDocument xml = new XmlDocument();
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}