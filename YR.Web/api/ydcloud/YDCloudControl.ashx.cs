using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Common.ZCloudUtil;
using YR.Web.api.api_class;
using YR.Web.api.zcloud.service;

namespace YR.Web.api.ydcloud
{
    /// <summary>
    /// 一动接口网关
    /// </summary>
    public class YDCloudControl : IHttpHandler
    {
        private static Log Logger = LogFactory.GetLogger(typeof(YDCloudControl));

        public void ProcessRequest(HttpContext context)
        {
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "text/plain";

            string clientData = "";
            try
            {
                if (context.Request.HttpMethod.ToLower() != "post")
                    throw new Exception("调用方式错误");

                var reader = new System.IO.StreamReader(context.Request.InputStream);
                clientData = reader.ReadToEnd(); 
                reader.Close();
                if (!string.IsNullOrEmpty(clientData))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string typeName = "StatusAction";
                    object obj = assembly.CreateInstance("YR.Web.api.ydcloud.service." + typeName);
                    IApiAction action = (IApiAction)obj;
                    //if (action == null)
                    //    return;

                    //调用相对应的接口方法
                    Hashtable args_ht = new Hashtable();
                    args_ht["json"] = clientData;
                    ApiResp result = action.Execute(args_ht);

                    Logger.Info("一动盒子接口网关上报数据(成功),访问ip:" + context.Request.UserHostAddress + ",参数:" + clientData + ",返回值:" + result);
                }
                else
                {
                    throw new Exception("请求参数为空");
                }
            }
            catch (Exception e)
            {
                Logger.Error("一动盒子接口网关上报数据(失败),访问ip:" + context.Request.UserHostAddress + ",参数:" + clientData + ",异常:" + e.Message);
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