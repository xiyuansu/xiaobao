using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Web.UI;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleMap : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string mapKey = SiteHelper.GetAppsetString("AMapBrowserKey");
            ViewState["MapKey"] = mapKey;
            ViewState["VehicleID"] = Request["VehicleID"];
            VehicleManager vehicleManager = new VehicleManager();
            Hashtable ht = vehicleManager.GetVehicleInfoByID(Request["VehicleID"]);
            ViewState["CityID"]=ht["CityID"].ToString();
            if (!this.IsPostBack)
            {

            }
        }
    }
}