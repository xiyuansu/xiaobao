using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetCode;
using YR.Common.DotNetJson;

namespace YR.Web.Ajax
{
    public partial class AjaxAppManage : System.Web.UI.Page
    {
        /// <summary>
        /// 判断请求字符串
        /// </summary>
        private string strResponse = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            string strType = RequestHelper.GetRequestString("type");
            switch (strType)
            {
                ////根据车辆ID获得车辆信息
                case "uploadimg":
                    UploadImg();
                    break;
            }

        }

        public void UploadImg()
        {
            HttpPostedFile file = Request.Files["FileData"];
            string uploadpath = Server.MapPath(Request["folder"] + "\\");
            if (file != null)
            {
                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }
                DateTime dtiime = System.DateTime.Now;
                string ftime = dtiime.ToString("yyyyMMddhhmmss");
                string[] pig = file.FileName.Split('.');
                string fname = "FXXK" + ftime + "." + pig[pig.Length - 1];
                file.SaveAs(uploadpath + fname);
                Response.Write(Request["folder"]+fname);
            }
            else
            {
                Response.Write("0");
            }
        }

    }
}