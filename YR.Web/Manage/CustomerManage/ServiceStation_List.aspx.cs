using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Busines.DAL;
using YR.Busines.IDAO;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.CustomerManage
{
    public partial class ServiceStation_List : PageBase//YR_ServiceStation
    {
        DataTable dtCustomer;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                this.BindPlatforms();
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
            ServiceStationManager vehicleManager = new ServiceStationManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = vehicleManager.GetServiceStationPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.PageIndex = pageIndex;
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
        }
        private void BindPlatforms()
        {
            CustomerManager hotelManager = new CustomerManager();
            dtCustomer= new DataTable();
            dtCustomer.Columns.Add("ID", Type.GetType("System.String"));
            dtCustomer.Columns.Add("Name", Type.GetType("System.String"));
            DataRow newRow = dtCustomer.NewRow();
            newRow["ID"] = "-1";
            newRow["Name"] = "";
            dtCustomer.Rows.Add(newRow);
            DataTable platformTable = hotelManager.GetAllCustomers();
            foreach (DataRow row in platformTable.Rows)
            {
                newRow = dtCustomer.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dtCustomer.Rows.Add(newRow);
            }
            selCustomer.DataSource = dtCustomer;
            selCustomer.DataTextField = "Name";
            selCustomer.DataValueField = "ID";
            selCustomer.DataBind();
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
                Label lblStation = e.Item.FindControl("lblCustomer") as Label;
                if (lblStation != null)
                {
                    string text = lblStation.Text;
                    if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtCustomer.Rows)
                        {
                            string stationID = dr["ID"] == null ? "" : dr["ID"].ToString();
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
            if (!string.IsNullOrEmpty(txtName.Value))
            {
                sb.Append(" and Name like @Name ");
                IList_param.Add(new SqlParam("@Name", '%' + txtName.Value.Trim() + '%'));
            }
            if (!string.IsNullOrEmpty(txtAddress.Value))
            {
                sb.Append(" and Address like @Address ");
                IList_param.Add(new SqlParam("@Address", '%' + txtAddress.Value.Trim() + '%'));
            }
            if (!string.IsNullOrEmpty(txtLongitude.Value))
            {
                sb.Append(" and (Longitude like @Longitude or Latitude like @Longitude)");
                IList_param.Add(new SqlParam("@Longitude", '%' + txtLongitude.Value.Trim() + '%'));
            }
            if (!string.IsNullOrEmpty(txtContacts.Value))
            {
                sb.Append(" and Contacts like @Contacts ");
                IList_param.Add(new SqlParam("@Contacts", '%' + txtContacts.Value.Trim() + '%'));
            }
            if (!string.IsNullOrEmpty(txtContactsTel.Value))
            {
                sb.Append(" and ContactsTel like @ContactsTel ");
                IList_param.Add(new SqlParam("@ContactsTel", '%' + txtContactsTel.Value.Trim() + '%'));
            }
            if (!string.IsNullOrEmpty(txtManager.Value))
            {
                sb.Append(" and Manager like @Manager ");
                IList_param.Add(new SqlParam("@Manager", '%' + txtManager.Value.Trim() + '%'));
            }
            if (!string.IsNullOrEmpty(txtManagerTel.Value))
            {
                sb.Append(" and ManagerTel like @ManagerTel ");
                IList_param.Add(new SqlParam("@ManagerTel", '%' + txtManagerTel.Value.Trim() + '%'));
            }
            if (selCustomer.Value != "-1")
            {
                sb.Append(" and CustomerID=@CustomerID ");
                IList_param.Add(new SqlParam("@CustomerID", selCustomer.Value));
            }

            if (selState.Value != "-1")
            {
                sb.Append(" and State=@State ");
                IList_param.Add(new SqlParam("@State", selState.Value));
            }
            // 起始创建时间
            /*if (!string.IsNullOrEmpty(txtStartCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCreateTime,CreateTime)>=0");
                IList_param.Add(new SqlParam("@StartCreateTime", txtStartCreateTime.Value));
            }*/
            // 结束创建时间
            if (!string.IsNullOrEmpty(txtEndCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,CreateTime,@EndCreateTime)>=0 ");
                IList_param.Add(new SqlParam("@EndCreateTime", txtEndCreateTime.Value));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txtName.Value = "";
            txtAddress.Value = "";
            txtEndCreateTime.Value = "";
            //txtStartCreateTime.Value = "";
            txtLongitude.Value = "";
            txtContacts.Value = "";
            txtContactsTel.Value = "";
            txtManager.Value = "";
            txtManagerTel.Value = "";
            selCustomer.Value = "-1";
            selState.Value = "-1";
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