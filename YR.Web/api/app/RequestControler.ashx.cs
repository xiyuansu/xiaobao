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
using YR.Common.DotNetCache;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;

namespace YR.Web.api.app
{
    /// <summary>
    /// app接口网关
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

            NameValueCollection myHeader = new NameValueCollection();
            myHeader = context.Request.Headers;
            int total = myHeader.Count;
            string equipmentID = "";
            bool hasEquipmentID = false;
            string strKey;
            for (int i = 0; i < total; i++)
            {
                strKey = myHeader.GetKey(i);
                if ("EquipmentID".Equals(strKey))
                {
                    equipmentID = myHeader.Get(strKey);
                    hasEquipmentID = true;
                }
            }
            //if (!hasEquipmentID)
            //{
            //    throw new Exception("参数校验失败");
            //}

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
                        if (typeName == "GetOrderCommentTags")
                        {
                            throw new Exception("无效接口");
                        }
                        /*if (typeName == "SendVerificationCode")
                        {
                            if (!string.IsNullOrEmpty(ip))
                            {
                                ICache Cache = CacheFactory.GetCache();
                                string keyid = "sms_send_times@" + ip;
                                int sms_send_times = 1;
                                if (string.IsNullOrEmpty(Cache.Get<string>(keyid)))
                                {
                                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                                    Cache.Set(keyid, sms_send_times, dt - DateTime.Now);
                                }
                                else
                                {
                                    string tims = Cache.Get<string>(keyid);
                                    int.TryParse(tims, out sms_send_times);
                                    sms_send_times += 1;
                                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                                    Cache.Set(keyid, sms_send_times, dt - DateTime.Now);
                                }
                                Cache.Dispose();
                                if (sms_send_times >= MaxSendTimes)
                                {
                                    Logger.Error("发送短信,访问ip:" + ip + ",参数:" + requestData);
                                    throw new Exception("短信发送已超过当天最大次数");
                                }
                            }
                        }*/
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        object obj = assembly.CreateInstance("YR.Web.api.app.privacy." + typeName);
                        IApiAction2 action = (IApiAction2)obj;

                        //调用相对应的接口方法
                        string result = action.Execute(ht);

                        if (ht["UID"] != null && ht["UID"].ToString().Trim().Length > 0)
                        {
                            string uid = ht["UID"].ToString();
                            context.Response.AddHeader("EquipmentID", new UserInfoManager().GetDeviceIDByUserID(uid));
                        }
                        string encrypt_data = SiteHelper.Encrypt(result, EncryptionKey);
                        context.Response.Write(encrypt_data);
                        if (!"GetNearVehicles".Equals(typeName) && !"GetNearParking".Equals(typeName))
                        {
                            Logger.Info("API接口调用(成功),访问ip:" + context.Request.UserHostAddress + equipmentID + ",参数:" + requestData + ",返回值:" + result);
                        }
                        else
                        {
                            Logger.Info("API接口调用(成功),访问ip:" + context.Request.UserHostAddress + equipmentID + ",参数:" + requestData);
                        }
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
                Logger.Error("API接口调用(失败),访问ip:" + context.Request.UserHostAddress + equipmentID + ",参数:" + requestData + ",异常:" + e.Message);
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