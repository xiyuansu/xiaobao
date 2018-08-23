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

namespace YR.Web.Manage.VehicleManage
{
    public partial class Vehicle_List : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                this.BindPlatforms();
                DataBindGrid();
                this.GetDepartmentID();
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
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            VehicleManager vehicleManager = new VehicleManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = vehicleManager.GetVehicleInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.PageIndex = pageIndex;
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
        }

        /// <summary>
        /// 绑定车辆平台
        /// </summary>
        private void BindPlatforms()
        {
            PlatformManager platformManager = new PlatformManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Platform", Type.GetType("System.String"));
            DataRow newRow= dt.NewRow();
            newRow["ID"] = "-1";
            newRow["Platform"] = "";
            dt.Rows.Add(newRow);
            DataTable platformTable = platformManager.GetAllPlatformInfo();
            foreach (DataRow row in platformTable.Rows)
            {
                newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Platform"] = row["Platform"];
                dt.Rows.Add(newRow);
            } 
            selPlatform.DataSource =dt;
            selPlatform.DataTextField = "Platform";
            selPlatform.DataValueField = "ID";
            selPlatform.DataBind();
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
                Label lblUser_Sex = e.Item.FindControl("lblUser_Sex") as Label;
                Label lblDeleteMark = e.Item.FindControl("lblDeleteMark") as Label;
                if (lblUser_Sex != null)
                {
                    string text = lblUser_Sex.Text;
                    text = text.Replace("1", "男士");
                    text = text.Replace("0", "女士");
                    lblUser_Sex.Text = text;

                    string textDeleteMark = lblDeleteMark.Text;
                    textDeleteMark = textDeleteMark.Replace("1", "<span style='color:Blue'>启用</span>");
                    textDeleteMark = textDeleteMark.Replace("2", "<span style='color:red'>停用</span>");
                    lblDeleteMark.Text = textDeleteMark;
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

            // 关键字
            if (!string.IsNullOrEmpty(txt_Search.Value))
            {
                sb.Append(" and (a.Name like @VehicleName or a.LicenseNumber like @VehicleName)");
                IList_param.Add(new SqlParam("@VehicleName", '%' + txt_Search.Value.Trim() + '%'));
            }

            // 车辆GPS编号
            if (!string.IsNullOrEmpty(txtVehicleGPSNum.Value))
            {
                sb.Append(" and a.VehicleGPSNum like @VehicleGPSNum ");
                IList_param.Add(new SqlParam("@VehicleGPSNum", '%' + txtVehicleGPSNum.Value.Trim() + '%'));
            }
            // 燃料方式
            if (selPlatform.Value != "-1")
            {
                sb.Append(" and a.PlatformId=@PlatformId ");
                IList_param.Add(new SqlParam("@PlatformId", selPlatform.Value));
            }
            // 使用状态
            if (selUseState.Value != "-1")
            {
                sb.Append(" and a.UseState=@UseState ");
                IList_param.Add(new SqlParam("@UseState", selUseState.Value));
            }
            // 车辆状态
            if (selVehicleState.Value != "-1")
            {
                sb.Append(" and a.VehicleState=@VehicleState ");
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
            // 在线状态
            if (selLiveState.Value != "-1")
            {
                if(selLiveState.Value == "1")
                    sb.Append(" and a.LastUpdateTime>Dateadd(n,-5, Getdate()) ");
                else if(selLiveState.Value == "2")
                    sb.Append(" and a.LastUpdateTime<=Dateadd(n,-5, Getdate()) ");
            }
            // 无单车辆过虑
            if (!string.IsNullOrEmpty(txtNoOrderDays.Value))
            {
                int days = int.Parse(txtNoOrderDays.Value);
                sb.Append(" and not exists(SELECT 1 FROM YR_Orders where VehicleID=a.ID and CreateTime>@CreateTime)");
                IList_param.Add(new SqlParam("@CreateTime", DateTime.Now.Date.AddDays(-days)));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txt_Search.Value = "";
            txtEndCreateTime.Value = "";
            txtStartCreateTime.Value = "";
            txtVehicleGPSNum.Value = "";
            selPlatform.Value = "-1";
            selUseState.Value = "-1";
            selUseState.Value = "-1";
            selLiveState.Value = "-1";
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            this.DataBindGrid(true);
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }
        // 重置
        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        protected void btnRefresh_ServerClick(object sender, EventArgs e)
        {
            this.DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        protected void lbtExport_Click(object sender, EventArgs e)
        {
            VehicleManager vehicleManager = new VehicleManager();
            int count = 0;
            int pageIndex = 1;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = vehicleManager.GetVehicleInfoPage(keyValue.Key, keyValue.Value, pageIndex, 1000000000, ref count);

            StringBuilder s = new StringBuilder();
            s.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            s.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");

            //写出列名
            s.AppendLine("<tr style=\"font-weight: bold; white-space: nowrap;\">");
            s.AppendLine("<td>车辆名称</td><td>型号</td><td>车牌号</td><td>电量</td><td>所属平台</td><td>所属城市</td><td>使用状态</td><td>车辆状态</td><td>电源状态</td><td>在线状态</td><td>最近更新</td>");
            s.AppendLine("</tr>");

            //写数据
            foreach (DataRow row in dt.Rows)
            {
                s.Append("<tr>");
                s.Append("<td>'").Append(row["Name"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["modelName"].ToString()).Append("</td>");
                s.Append("<td>'").Append(row["LicenseNumber"].ToString());
                s.Append("<td>'").Append(row["Electricity"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["Platform"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["VehicleCityName"].ToString()).Append("</td>");
                s.Append("<td>").Append(Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleUseState), Convert.ToInt32(row["UseState"].ToString()))).Append("</td>");
                s.Append("<td>").Append(Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleState), Convert.ToInt32(row["VehicleState"].ToString()))).Append("</td>");
                s.Append("<td>").Append(row["LockState"].ToString() == "lock" ? "已关" : "已开").Append("</td>");
                s.Append("<td>").Append(row["LightState"].ToString() == "on" ? "已开灯" : "已关灯").Append("</td>");
                s.Append("<td>").Append(int.Parse(row["diffminutes"].ToString()) < 5 ? "在线" : "离线").Append("</td>");
                s.Append("<td>").Append(row["LastUpdateTime"].ToString()).Append("</td>");
                s.AppendLine("</tr>");
            }
            s.AppendLine("</table>");
            this.Page.Response.ContentType = "application/vnd.ms-excel";
            this.Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
            this.Page.Response.Charset = "utf-8";
            string filename = "车辆列表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(filename));
            this.Page.Response.Write(s.ToString());
            this.Page.Response.End();
        }
    }
}