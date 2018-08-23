using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetCode;

namespace YR.Web.api.weapp
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

            string paramstr = "type=" + this.methodTxt.Text + "&Client=android&" + this.argsTxt.Text;
            string apiurl = string.Format("{0}/api/weapp/RequestControler.ashx", apiUrl);
            string result = HttpUtil.Post(apiurl, paramstr, "utf-8");

            this.resultLit.Text = result;
        }
    }
}