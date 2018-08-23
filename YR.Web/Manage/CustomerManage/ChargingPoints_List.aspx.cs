using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.CustomerManage
{
    public partial class ChargingPoints_List : PageBase          //YR_ChargingPoints
    {
        public StringBuilder str_tableTree = new StringBuilder();
        DataTable dtStations;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                this.BindStations();
                DataBindGrid();
            }
        }

        /// <summary>
        /// 绑定数据，分页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void pager_PageChanged(object sender, EventArgs e)
        {
            DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "callback();", true);
        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            ChargingPointsManager cpManager = new ChargingPointsManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = cpManager.GetChargingPointPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.PageIndex = pageIndex;
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
        }

        private void BindStations()
        {
            ServiceStationManager ssManager = new ServiceStationManager();
            dtStations = new DataTable();
            dtStations.Columns.Add("ID", Type.GetType("System.String"));
            dtStations.Columns.Add("Name", Type.GetType("System.String"));
            DataRow newRow = dtStations.NewRow();
            newRow["ID"] = "-1";
            newRow["Name"] = "全部";
            dtStations.Rows.Add(newRow);
            DataTable platformTable = ssManager.GetAllServiceStations();
            foreach (DataRow row in platformTable.Rows)
            {
                newRow = dtStations.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dtStations.Rows.Add(newRow);
            }
            selStation.DataSource = dtStations;
            selStation.DataTextField = "Name";
            selStation.DataValueField = "ID";
            selStation.DataBind();
        }
        /// <summary>
        /// 绑定后激发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblStation = e.Item.FindControl("lblStation") as Label;
                if (lblStation != null)
                {
                    string text = lblStation.Text;
                    if (dtStations!=null&&dtStations.Rows.Count>0)
                    {
                        foreach (DataRow dr in dtStations.Rows)
                        {
                            string stationID=dr["ID"]==null?"":dr["ID"].ToString();
                            if (text.Equals(stationID))
                            {
                                text = dr["Name"].ToString();
                                lblStation.Text = text;
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化查询条件
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<StringBuilder, IList<SqlParam>> InitCondition()
        {
            StringBuilder sb = new StringBuilder();
            List<SqlParam> IList_param = new List<SqlParam>();
            // 关键字（充电桩名称、GPS号 、手机号）Name,IPAddr,IMEI,Address
            if (!string.IsNullOrEmpty(txt_Search.Value))
            {
                sb.Append(" and (Name like @name or IPAddr like @name or IMEI like @name or Address like @name or Phone like @name) ");
                IList_param.Add(new SqlParam("@name", '%' + txt_Search.Value.Trim() + '%'));
            }
            // 手机号
            if (!string.IsNullOrEmpty(txtVehicleMobile.Value))
            {
                sb.Append(" and Phone like @VehicleMobile ");
                IList_param.Add(new SqlParam("@VehicleMobile", '%' + txtVehicleMobile.Value.Trim() + '%'));
            }
            // 所属充电站
            if (selStation.Value != "-1")
            {
                sb.Append(" and StationID=@StationID ");
                IList_param.Add(new SqlParam("@StationID", selStation.Value));
            }
            // 使用状态
            if (selUseState.Value != "-1")
            {
                sb.Append(" and UseState=@UseState ");
                IList_param.Add(new SqlParam("@UseState", selUseState.Value));
            }
            // 车辆状态
            if (selVehicleState.Value != "-1")
            {
                sb.Append(" and State=@VehicleState ");
                IList_param.Add(new SqlParam("@VehicleState", selVehicleState.Value));
            }
            // 起始创建时间
            if (!string.IsNullOrEmpty(txtStartCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCreateTime,CreateTime)>=0");
                IList_param.Add(new SqlParam("@StartCreateTime", txtStartCreateTime.Value));
            }
            // 结束创建时间
            if (!string.IsNullOrEmpty(txtEndCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,a.CreateTime,@EndCreateTime)>=0 ");
                IList_param.Add(new SqlParam("@EndCreateTime", txtEndCreateTime.Value));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txt_Search.Value = "";
            txtVehicleMobile.Value = "";
            txtEndCreateTime.Value = "";
            txtStartCreateTime.Value = "";
            selStation.Value = "-1";
            selUseState.Value = "-1";
            selVehicleState.Value = "-1";
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            this.DataBindGrid(true);
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "callback();", true);
        }
        // 重置
        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "callback();", true);
        }

        protected void btnRefresh_ServerClick(object sender, EventArgs e)
        {
            this.DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "callback();", true);
        }
    }
}