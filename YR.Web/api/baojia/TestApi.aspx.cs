using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetCode;

namespace YR.Web.api.baojia
{
    public partial class TestApi : System.Web.UI.Page
    {
        private string apiUrl = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            apiUrl = string.Format("http://{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.methodTxt.Text))
            {
                this.resultLit.Text = "调用方法未填写，请填写后重试!";
                return;
            }
            if (string.IsNullOrEmpty(this.argsTxt.Text))
            {
                this.resultLit.Text = "调用参数未填写，请填写后重试!";
                return;
            }

            string paramstr = "action=" + this.methodTxt.Text + "&" + this.argsTxt.Text;
            Hashtable para_ht=SiteHelper.GetHashTableByQueryString(paramstr);
            paramstr += "&sign=" + GetSign(para_ht, "6c46ea43a15e660bb337b73b9e2c0b2d");
            string apiurl = string.Format("{0}/api/baojia/gateway.ashx", apiUrl);
            string result = HttpUtil.Post(apiurl, paramstr, "utf-8");

            this.resultLit.Text = result;
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

        private string GetSign(Hashtable ht, string appkey)
        {
            try
            {
                SortedDictionary<string, string> dict = new SortedDictionary<string, string>();

                foreach (string key in ht.Keys)
                {
                    if (key != "sign" && key!="action")
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

                return sign;
            }
            catch
            {
                return null;
            }
        }
    }
}