using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YR.Web.Frame
{
    public partial class MainDefault : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!this.IsPostBack)
            {
                this.img_logo.ImageUrl = Asiasofti.SmartVehicle.Common.SiteHelper.GetAppsetString("webLogo");
            }
        }
    }
}