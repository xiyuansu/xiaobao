using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using YR.Busines.IDAO;
using YR.Common.DotNetBean;
using YR.Busines;
using YR.Common.DotNetUI;
using YR.Common.DotNetData;
using YR.Common.DotNetCode;
using YR.Common.DotNetEncrypt;
using YR.Busines.DAL;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.CustomerManage
{
    public partial class ChargingPoints_Form : PageBase
    {
        ChargingPointsManager manager = new ChargingPointsManager();

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
                this.BindStations();
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
            Hashtable ht = manager.GetChargingPointByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
        }

        private void BindStations()
        {
            ServiceStationManager station = new ServiceStationManager();
            selStation.DataSource = station.GetAllServiceStations();
            selStation.DataTextField = "Name";
            selStation.DataValueField = "ID";
            selStation.DataBind();
        }

        /// <summary>
        /// 验证是否存在
        /// </summary>
        /// <param name="pkName">对象主键</param>
        /// <param name="Obj_id">对象主键值</param>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public string GetChecked(string pkName, string Obj_Val, DataTable dt)
        {
            StringBuilder strSql = new StringBuilder();
            dt = DataTableHelper.GetNewDataTable(dt, "" + pkName + " = '" + Obj_Val + "'");
            if (DataTableHelper.IsExistRows(dt))
                return "checked=\"checked\"";
            else
                return "";
        }

        #region 保存事件
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                Hashtable ht = new Hashtable();
                #region 初始化车辆信息
                ht["Name"] = Name.Value;
                ht["IPAddr"] = IPAddr.Value;
                ht["IMEI"] = IMEI.Value;
                ht["Longitude"] = Longitude.Value;
                ht["Latitude"] = Latitude.Value;
                ht["Address"] = Address.Value;
                ht["StationID"] = selStation.Value;
                ht["State"] = State.Value;
                ht["UseState"] = UseState.Value;
                ht["Phone"] = Phone.Value;
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                    ht["DeleteMark"] = "1";// 1 未删除，0 已删除
                }
                #endregion

                bool returnValue = manager.AddOrEditChargingPoint(ht, _key);

                if (returnValue)
                {
                    ShowMsgHelper.AlertMsgNoFresh("操作成功！");
                }
                else
                {
                    ShowMsgHelper.Alert_Error("操作失败！");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}