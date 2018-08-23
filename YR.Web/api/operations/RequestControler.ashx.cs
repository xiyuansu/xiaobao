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

namespace YR.Web.api.partner_v2
{
    /// <summary>
    /// 运维接口网关
    /// </summary>
    public class RequestControler : IHttpHandler
    {
        private static Log Logger = LogFactory.GetLogger(typeof(RequestControler));

        private string ClientEncryptionKey = SiteHelper.GetAppsetString("ClientEncryptionKey");

        public string EncryptionKey = SiteHelper.GetAppsetString("EncryptionKey");

        public void ProcessRequest(HttpContext context)
        {
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "text/plain";

            string requestData = "";
            try
            {
                string clientData = context.Request["data"];
                if (clientData != null && clientData.Length > 0)
                {
                    clientData = clientData.Replace(",", "+");
                    //解密字符串
                    requestData = SiteHelper.Decrypt(clientData, ClientEncryptionKey);
                    if (requestData.Length > 0)
                    {
                        Hashtable ht = SiteHelper.GetHashTableByQueryString(requestData);
                        if (ht["type"] == null || ht["type"].ToString().Trim().Length <= 0)
                            throw new Exception("参数不完整");
                        if ((ht["UID"] == null || ht["UID"].ToString().Trim().Length <= 0) && ht["type"].ToString().Trim() != "Login")
                            throw new Exception("参数不完整");

                        string typeName = ht["type"].ToString();
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        object obj = assembly.CreateInstance("YR.Web.api.operations.privacy." + typeName);
                        IApiAction2 action = (IApiAction2)obj;

                        //调用相对应的接口方法
                        string result = action.Execute(ht);

                        string encrypt_data = SiteHelper.Encrypt(result, EncryptionKey);
                        context.Response.Write(encrypt_data);

                        Logger.Info("运维API接口调用(成功),访问ip:" + context.Request.UserHostAddress + ",参数:" + requestData + ",返回值:" + result);
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
                string encrypt_data = SiteHelper.Encrypt(result, EncryptionKey);
                context.Response.Write(encrypt_data);

                Logger.Error("运维API接口调用(失败),访问ip:" + context.Request.UserHostAddress + ",参数:" + requestData + ",异常:" + e.Message);
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