using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;
using YR.Common.DotNetConfig;

namespace YR.Common.DotNetSMS
{
    /// <summary>
    /// 和信通短信接口
    /// </summary>
    public class HXTSMS:ISMS
    {
        public bool SendMessage(string telphone, string message)
        {
            try
            {
                bool result = false;
                string apiurl = "http://123.57.51.191:8888/sms.aspx";
                string userid="845";
                string account="bjcrs";
                string password="crs@2015";
                string sendtime="";
                string action="send";
                string extno="";
                string paramstr = string.Format("userid={0}&account={1}&password={2}&mobile={3}&content={4}&sendTime={5}&action={6}&extno={7}",userid,account,password,telphone,HttpUtility.UrlEncode(message),sendtime,action,extno);
                string retstr = HttpUtil.Post(apiurl, paramstr, "utf-8");
                dynamic retObj = new DynamicXml(retstr);
                if ("Success".CompareTo(retObj.returnstatus.Value)==0)
                    result = true;

                return result;
            }
            catch
            {
                return false;
            }
        }

        public bool SendAlarmMessage(string telphone, string message)
        {
            string apiurl = "http://api.zthysms.com/sendSms.do";
            try
            {
                bool result = false;
                string account = ConfigHelper.GetAppSettings("ZTSMS_Business_UserName");
                string pwd = ConfigHelper.GetAppSettings("ZTSMS_Business_Password");
                string tkey = DateTime.Now.ToString("yyyyMMddHHmmss");
                string password = "";
                string paramstr = "username=" + account + "&password=" + password + "&tkey=" + tkey + "&mobile=" + telphone + "&content=" + message;
                string retstr = HttpUtil.Post(apiurl, paramstr, "utf-8");
                if (!string.IsNullOrEmpty(retstr))
                {
                    string[] array = retstr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (array.Length == 2 && array[0] == "1")
                    {
                        result = true;
                    }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        public bool SendAlarm(string telphone, string vehicleGPSNum, string vehicleName, string type)
        {
            string company = ConfigHelper.GetAppSettings("SMS_CompanyName");
            string message = string.Format("【{0}】车辆{1}，{2}发生{3}报警，请尽快排查。", company, vehicleName, vehicleGPSNum, type);
            return SendAlarmMessage(telphone, message);
        }

        /*******************************************短信发送模板*********************************************************
        尊敬的用户，您的验证码是#code#。请不要把验证码泄露给其他人。如非本人操作，请忽略本短信。 
         **********************************************************************************************************************/

        public bool SendCheckCode(string telphone, string code)
        {
            string message = string.Format("尊敬的用户，您的验证码是{0}。请不要把验证码泄露给其他人。如非本人操作，请忽略本短信。 ", code);
            return SendMessage(telphone, message);
        }

    }
}