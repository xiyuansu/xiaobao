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

namespace YR.Web.api.zcloud
{
    /// <summary>
    /// 自由box接口网关
    /// </summary>
    public class ZcloudControl : IHttpHandler
    {
        private static Log Logger = LogFactory.GetLogger(typeof(ZcloudControl));

        /// <summary>
        /// 无需返回数据命令列表
        /// </summary>
        private static string[] Not_Response_Cmd = { "authenticate", "offLine", "doorControlResp", "keyCardAction" };

        public void ProcessRequest(HttpContext context)
        {
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "text/plain";

            Hashtable clientData = new Hashtable();
            string cmd = null;
            try
            {
                if (context.Request.HttpMethod.ToLower() != "post")
                    throw new Exception("调用方式错误");

                clientData = GetRequestData(context);
                if (clientData != null && clientData.Keys.Count > 0)
                {
                    if (!ZCloudSignUtil.verify(ZCloudCore.hashtableToDictionary(clientData), ZCloudConfig.md5_key))
                        throw new Exception("验签失败");

                    if (clientData["cmd"] != null)
                        cmd = clientData["cmd"].ToString();
                    if (string.IsNullOrEmpty(cmd))
                        throw new Exception("收到网关发来的通知，但是没有找到命令字");
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string typeName = cmd.Substring(0, 1).ToUpper() + cmd.Substring(1, cmd.Length - 1) + "Action";
                    object obj = assembly.CreateInstance("YR.Web.api.zcloud.service." + typeName);
                    IApiAction action = (IApiAction)obj;
                    //if (action == null)
                    //    return;

                    //调用相对应的接口方法
                    ApiResp result = action.Execute(clientData);

                    if (!Not_Response_Cmd.Contains(cmd))
                    {
                        Hashtable result_ht = new Hashtable();
                        result_ht.Add("rtCode", "0");
                        string sign = ZCloudSignUtil.getSign(ZCloudCore.hashtableToDictionary(result_ht), ZCloudConfig.md5_key);
                        result_ht.Add("sign", sign);
                        context.Response.Write(JsonConvert.SerializeObject(result_ht));
                    }
                    Logger.Info("自由box接口网关上报数据(成功),访问ip:" + context.Request.UserHostAddress + ",参数:" + JsonConvert.SerializeObject(clientData) + ",返回值:" + result);
                }
                else
                {
                    throw new Exception("请求参数为空");
                }
            }
            catch (Exception e)
            {
                if (!Not_Response_Cmd.Contains(cmd))
                {
                    Hashtable result_ht = new Hashtable();
                    result_ht.Add("rtCode", "1");
                    string sign = ZCloudSignUtil.getSign(ZCloudCore.hashtableToDictionary(result_ht), ZCloudConfig.md5_key);
                    result_ht.Add("sign", sign);
                    context.Response.Write(JsonConvert.SerializeObject(result_ht));
                }
                Logger.Error("自由box接口网关上报数据(失败),访问ip:" + context.Request.UserHostAddress + ",参数:" + JsonConvert.SerializeObject(clientData) + ",异常:" + e.Message);
            }
        }

        private Hashtable GetRequestData(HttpContext context)
        {
            Hashtable ht = new Hashtable();
            NameValueCollection coll = context.Request.Form;
            //NameValueCollection coll = context.Request.Params;
            string[] requestItem = coll.AllKeys;
            for (int i = 0; i < requestItem.Length; i++)
            {
                ht.Add(requestItem[i], coll[requestItem[i]]);
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