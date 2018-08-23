using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using YR.Common.DotNetCode;
using YR.Common.DotNetConfig;
using YR.Common.DotNetLog;

namespace YR.Common.DotNetSMS
{
    /// <summary>
    /// 助通科技短信接口
    /// </summary>
    public class ZTSMS:ISMS
    {
        private static Log Logger = LogFactory.GetLogger(typeof(ZTSMS));
        public bool SendMessage(string telphone, string message)
        {
            //string apiurl = "http://www.api.zthysms.com:8088/sendSms.do";
            string apiurl = "http://www.yzmsms.cn/sendSmsYZM.do";
            try
            {
                bool result = false;
                string account =ConfigHelper.GetAppSettings("ZTSMS_UserName");
                string pwd =ConfigHelper.GetAppSettings("ZTSMS_Password");
                string tkey = DateTime.Now.ToString("yyyyMMddHHmmss");
                string password = MD5Encrypt(MD5Encrypt(pwd).ToLower() + tkey).ToLower();
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

        public bool SendAlarmMessage(string telphone, string message)
        {
            string apiurl = "http://api.zthysms.com/sendSms.do";
            try
            {
                bool result = false;
                string account = ConfigHelper.GetAppSettings("ZTSMS_Business_UserName");
                string pwd = ConfigHelper.GetAppSettings("ZTSMS_Business_Password");
                string tkey = DateTime.Now.ToString("yyyyMMddHHmmss");
                string password = MD5Encrypt(MD5Encrypt(pwd).ToLower() + tkey).ToLower();
                string paramstr = "username=" + account + "&password=" + password + "&tkey=" + tkey + "&mobile=" + telphone + "&content=" + message;
                string retstr = HttpUtil.Post(apiurl, paramstr, "utf-8");
                if (!string.IsNullOrEmpty(retstr))
                {
                    string[] array = retstr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (array.Length == 2 && array[0] == "1")
                    {
                        result = true;
                    }
                    else
                    {
                        Logger.Error(string.Format("发送短信{0}，{1}，{2}", telphone, message, retstr));
                    }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        /*******************************************短信发送模板*********************************************************
        尊敬的用户，您的验证码是#code#。请不要把验证码泄露给其他人。如非本人操作，请忽略本短信。 
         **********************************************************************************************************************/

        public bool SendCheckCode(string telphone, string code)
        {
            string company=ConfigHelper.GetAppSettings("SMS_CompanyName");
            string message = string.Format("【{0}】尊敬的用户，您的验证码是{1}。请不要把验证码泄露给其他人。如非本人操作，请忽略本短信", company, code);
            return SendMessage(telphone, message);
        }

        private static string MD5Encrypt(string strText)
        {
            byte[] result = Encoding.Default.GetBytes(strText); 
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }

    }
}