using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YR.Web.upfile
{
    public partial class upstart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request["UPType"] != null)
                {
                    this.Session["UPType"] = Request["UPType"].ToString();
                }
            }
        }
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static object GetUploadStatus()
        {
            string id = string.Empty;
            if (HttpContext.Current.Session["ID"] != null)
            {
                id = HttpContext.Current.Session["ID"].ToString();
            }

            int percentComplete = 100;
            string message = string.Format("上传 {0} ... {1} of {2} KB", "", "", "");

            //  返回百分比
            return new { percentComplete = percentComplete, message = message, id = id };

        }
    }
}