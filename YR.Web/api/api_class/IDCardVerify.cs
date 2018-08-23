using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;

namespace YR.Web.api.api_class
{
    /// <summary>
    /// 验证身份证类，第三方提供,www.nuozhengtong.com
    /// </summary>
    public class IDCardVerify
    {
        /// <summary>
        /// 验证身份证与姓名是否一致
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="idcard">身份证号码</param>
        /// <returns>返回值，其中包括的值有(code：0,访问正常，-1,访问异常，data:1,验证一致,2,不一致,message:文本消息)</returns>
        public ObjectApiResp VerifyCard(string name,string idcard)
        {
            string mallid = SiteHelper.GetAppsetString("IDCardVerfy_MallID");
            string appkey = SiteHelper.GetAppsetString("IDCardVerfy_AppKey");
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "-1"; 
            try
            {
                string apiurl = string.Format("http://121.41.42.121:8080/v2/id-server");
                string tm = DateTime.Now.Ticks.ToString();
                tm = DateTime.Now.Ticks.ToString().Substring(0,13);
                string paramstr = string.Format("mall_id={0}&realname={1}&idcard={2}&tm={3}&sign={4}", mallid, name, idcard.ToLower(), tm, MD5(mallid + name + idcard.ToLower() + tm + appkey));
                string retstr = HttpUtil.Get(apiurl, paramstr, "utf-8");
                dynamic retObj = DynamicJson.Parse(retstr);
                resp.Message = retObj.data.message;
                if(retObj.status=="2001")
                {
                    resp.Code = "0";
                    if(retObj.data.code=="1000")
                    {
                        resp.Data = "1";
                    }
                    else if (retObj.data.code == "1001")
                    {
                        resp.Data = "2";
                    }
                    else
                    {
                        resp.Code = "-1";
                    }
                }
                return resp;
            }
            catch
            {
                resp.Code = "-1";
                resp.Message = "身份证验证系统异常";
                return resp;
            }
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

    }
}