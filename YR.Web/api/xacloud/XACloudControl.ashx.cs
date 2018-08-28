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
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Common.ZCloudUtil;
using YR.Web.api.api_class;
using YR.Web.api.xacloud.service;
using YR.Web.api.zcloud.service;

namespace YR.Web.api.xacloud
{
    /// <summary>
    /// 小安盒子接口网关
    /// </summary>
    public class XACloudControl : IHttpHandler
    {
        private static Log Logger = LogFactory.GetLogger(typeof(XACloudControl));


        public void ProcessRequest(HttpContext context)
        {
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "text/plain";

            string clientData = "";
            try
            {
                if (context.Request.HttpMethod.ToLower() != "post")
                {
                    throw new Exception("调用方式错误");
                }

                var reader = new System.IO.StreamReader(context.Request.InputStream);
                clientData = reader.ReadToEnd();
                reader.Close();
                if (!string.IsNullOrEmpty(clientData))
                {
                    clientData = clientData.Trim();
                    string cmd = "";
                    dynamic jsonObj = DynamicJson.Parse(clientData);
                    if (jsonObj.IsDefined("cmd"))
                    {
                        cmd = jsonObj.cmd;
                    }
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string typeName = cmd.Substring(0, 1).ToUpper() + cmd.Substring(1, cmd.Length - 1) + "Action";
                    object obj = assembly.CreateInstance("YR.Web.api.xacloud.service." + typeName);
                    IApiAction action = (IApiAction)obj;
                    //调用相对应的接口方法
                    Hashtable args_ht = new Hashtable();
                    args_ht["json"] = clientData;
                    ApiResp result = action.Execute(args_ht);
                    
                    Logger.Info("小安盒子接口网关上报数据," + result.Code + ",访问ip:" + context.Request.UserHostAddress + ",参数:" + clientData + ",返回值:" + JsonConvert.SerializeObject(result));
                    context.Response.Write(JsonConvert.SerializeObject(result));
                }
                else
                {
                    Logger.Error("小安盒子接口网关上报数据失败,请求参数为空");
                    throw new Exception("请求参数为空");
                }
            }
            catch (Exception e)
            {
                ApiResp result = new ApiResp();
                result.Code = "-1";
                result.Message = e.Message;
                Logger.Error("小安盒子接口网关上报数据失败,访问ip:" + context.Request.UserHostAddress + ",参数:" + clientData + ",异常:" + e);
                context.Response.Write(JsonConvert.SerializeObject(result));
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