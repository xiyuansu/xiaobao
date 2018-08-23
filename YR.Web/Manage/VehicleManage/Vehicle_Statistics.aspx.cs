using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using YR.Common.DotNetUI;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.VehicleManage
{
    public partial class Vehicle_Statistics : PageBase
    {
        public DataTable dt = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }


        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            string startDate = txtStartDate.Value + " 00:00:00";
            string endDate = txtEndDate.Value + " 23:59:59";
            OrdersManager ordersManager = new OrdersManager();
            dt = ordersManager.VehicleStatistics(startDate, endDate);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
        }

        /// <summary>
        /// 绑定后激发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void ClearInput()
        {
            txtStartDate.Value = "";
            txtEndDate.Value = "";
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            string startDate = txtStartDate.Value;
            string endDate = txtEndDate.Value;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime dtStart = Convert.ToDateTime(startDate);
                DateTime dtEnd = Convert.ToDateTime(endDate);
                if (DateTime.Compare(dtEnd, dtStart) >= 0)
                {
                    this.DataBindGrid(true);
                    ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);

                }
            }
        }

        // 重置
        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
        }

        protected void lbtExport_Click(object sender, EventArgs e)
        {
            string startDate = txtStartDate.Value + " 00:00:00";
            string endDate = txtEndDate.Value + " 23:59:59";
            OrdersManager ordersManager = new OrdersManager();
            dt = ordersManager.VehicleStatistics(startDate, endDate);
            if (dt != null && dt.Rows.Count > 0)
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                s.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
                //写出列名
                s.AppendLine("<tr>");
                s.AppendLine("<td>日期</td><td>支付订单数</td><td>用车数</td>");
                s.AppendLine("</tr>");

                //写数据
                foreach (DataRow row in dt.Rows)
                {
                    s.Append("<tr>");
                    s.Append("<td>").Append(row["PayTime"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["OrderCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["VehicleCount"].ToString()).Append("</td>");
                    s.AppendLine("</tr>");
                }
                s.AppendLine("</table>");
                this.Page.Response.ContentType = "application/vnd.ms-excel";
                this.Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
                this.Page.Response.Charset = "Utf-8";
                this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=小宝出行用车统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                this.Page.Response.Write(s.ToString());
                this.Page.Response.End();
            }
        }
    }
}