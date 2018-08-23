using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Asiasofti.SmartVehicle.Manager;
using System.Collections;

namespace YR.Web.Manage.InformationManage
{
    public partial class Notification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.InitPage();
        }

        private void InitPage()
        {
            // 消息
            MessageManager messageManager = new MessageManager();
            Hashtable ht = messageManager.GetMessageInfoByID(Request["id"]);
            if (ht != null && ht.Count > 0)
            {
                this.divTitle.InnerHtml = ht["MESSAGETITLE"].ToString();
                this.divDate.InnerHtml = DateTime.Parse(ht["RELEASETIME"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                this.divContent.InnerHtml = ht["MESSAGECONTENT"].ToString();
            }
        }
    }
}