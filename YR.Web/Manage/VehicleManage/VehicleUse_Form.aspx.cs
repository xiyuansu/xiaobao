using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleUse_Form : System.Web.UI.Page
    {
        private VehicleManager manager = new VehicleManager();

        private OrdersManager orderManager = new OrdersManager();

        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(_key))
                {
                    InitData();
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            Hashtable ht = manager.GetVehicleUseInfoByID(_key);
            if (ht != null && ht.Count > 0)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
        }

    }
}