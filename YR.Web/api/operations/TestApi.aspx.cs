using Asiasofti.SmartVehicle.Common;
using System;
using System.Net;
using System.Web;

namespace YR.Web.api.partner_v2
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
            
            string data = "type="+this.methodTxt.Text+"&Client=android&"+this.argsTxt.Text;
            data = SiteHelper.Encrypt(data, "qazwsxedcrfvtgbyhnujmikoyhbgtrew").Replace("+", ",");
            HttpWebRequest webrequest = WebRequest.Create(string.Format("{0}/api/operations/RequestControler.ashx?data={1}",apiUrl, data)) as HttpWebRequest;
            WebResponse wr = webrequest.GetResponse();
            System.IO.Stream stream = wr.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string result = sr.ReadToEnd();
            result = SiteHelper.Decrypt(result, "qazwsxedcrfvtgbyhnujmikoyhbgtrew");

            this.resultLit.Text = result;
        }
    }
}