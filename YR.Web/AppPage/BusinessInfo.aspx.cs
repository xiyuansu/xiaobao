using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Asiasofti.SmartVehicle.Manager;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;

namespace YR.Web.AppPage
{
    public partial class BusinessInfo : System.Web.UI.Page
    {
        /// <summary>
        /// 字段类型
        /// </summary>
        private int FieldType
        {
            get
            {
                int fieldType = 0;
                int.TryParse(Request["field"], out fieldType);
                return fieldType;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.InitPage();
        }

        private void InitPage()
        {
            Hashtable ht = new Hashtable();
            string fieldName = EnumHelper.GetEnumShowName(typeof(BusinessFieldType), FieldType);

            if (Request["type"] == BusinessType.Vehicle.GetHashCode().ToString())
            {
                // 车辆详细（介绍、价格体系、用户须知）
                VehicleManager vehicleManager = new VehicleManager();
                ht = vehicleManager.GetVehicleInfoByID(Request["id"]);
            }
            else if (Request["type"] == BusinessType.ChargingPies.GetHashCode().ToString())
            {
                // 充电桩/停车场
                ServiceAreaManager chargingPiesManager = new ServiceAreaManager();
                ht = chargingPiesManager.GetInfoByID(Request["id"]);
            }
            else if (Request["type"] == BusinessType.Pits.GetHashCode().ToString())
            {
                // 维修站
                PitsManager pitsManager = new PitsManager();
                ht = pitsManager.GetPitsInfoByID(Request["id"]);
            }
            if (ht != null && ht.Count > 0)
            {
                this.divContent.InnerHtml = ht[fieldName].ToString();
            }
        }
    }
}