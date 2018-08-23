using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;


namespace YR.Web.Manage.UserManage
{
    public partial class UserDepositReturn_List : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
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
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid()
        {
            DepositReturnManager depositManager = new DepositReturnManager();
            int count = 0;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = depositManager.GetPage(keyValue.Key, keyValue.Value, PageControl1.PageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
        }

        /// <summary>
        /// 初始化查询条件
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<StringBuilder, IList<SqlParam>> InitCondition()
        {
            StringBuilder sb = new StringBuilder();
            List<SqlParam> IList_param = new List<SqlParam>();

            if (!string.IsNullOrEmpty(txtTel.Value))
            {
                sb.Append(" and b.BindPhone like @BindPhone ");
                IList_param.Add(new SqlParam("@BindPhone", '%' + txtTel.Value.Trim() + '%'));
            }
            if (selState.Value != "-1")
            {
                sb.Append(" and a.State=@State ");
                IList_param.Add(new SqlParam("@State", selState.Value));
            }
            if (!string.IsNullOrEmpty(txtStartCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCreateTime,a.CreateTime)>=0");
                IList_param.Add(new SqlParam("@StartCreateTime", txtStartCreateTime.Value));
            }
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
        private void ClearInput()
        {
            txtTel.Value = "";
            selState.Value = "-1";
            txtStartCreateTime.Value = "";
            txtEndCreateTime.Value = "";
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        public string GetStateTxt(string state)
        {
            string str = "";
            switch(state)
            {
                case "1":
                    str = "待处理";
                    break;
                case "2":
                    str = "退款成功";
                    break;
                case "3":
                    str = "退款失败";
                    break;
                default:
                    break;
            }
            return str;
        }

        protected void lbtExport_Click(object sender, EventArgs e)
        {
            DepositReturnManager depositManager = new DepositReturnManager();
            int count = 0;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = depositManager.GetPage(keyValue.Key, keyValue.Value, PageControl1.PageIndex, 1000000000, ref count);

            StringBuilder s = new StringBuilder();
            s.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            s.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");

            //写出列名
            s.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
            s.AppendLine("<td>用户姓名/td><td>手机号</td><td>押金金额</td><td>押金支付方式</td><td>押金支付交易号</td><td>申请状态</td><td>创建时间</td>");
            s.AppendLine("</tr>");

            //写数据
            foreach (DataRow row in dt.Rows)
            {
                s.Append("<tr>");
                s.Append("<td>").Append(row["RealName"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["BindPhone"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["DepositMoney"].ToString()).Append("</td>");
                s.Append("<td>").Append(EnumHelper.GetEnumShowName(typeof(UserFinancialOperatorWay), Convert.ToInt32(row["DepositPayWay"].ToString()))).Append("</td>");
                s.Append("<td>'").Append(row["DepositTradeNo"].ToString()).Append("</td>");
                s.Append("<td>").Append(GetStateTxt(row["State"].ToString())).Append("</td>");
                s.Append("<td>").Append(row["CreateTime"].ToString()).Append("</td>");
                s.AppendLine("</tr>");
            }
            s.AppendLine("</table>");
            this.Page.Response.ContentType = "application/vnd.ms-excel";
            this.Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
            this.Page.Response.Charset = "Utf-8";
            this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=押金退款列表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            this.Page.Response.Write(s.ToString());
            this.Page.Response.End();
        }
    }
}