using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.SystemInfo;
using YR.Web.App_Code;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleMonitor : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string mapKey = SiteHelper.GetAppsetString("AMapBrowserKey");
            ViewState["MapKey"] = mapKey;
            if(!this.IsPostBack)
            {

            }
        }
    }
}