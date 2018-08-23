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
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;

namespace YR.Web.Manage.UserManage
{
    public partial class Orders_List : PageBase
    {
        public StringBuilder str_tableTree = new StringBuilder();
        YR_UserInfo_IDAO user_idao = new YR_UserInfo_Dal();
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
        private void DataBindGrid(bool isQuery = false)
        {
            OrdersManager ordersManager = new OrdersManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();

            DataTable dt = ordersManager.GetOrdersInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);

            this.PageControl1.PageIndex = pageIndex;
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
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

        public string GetRealNameCertification(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "1":
                        return "未认证";
                    case "2":
                        return "提交申请";
                    case "3":
                        return "审核失败";
                    case "4":
                        return "已认证";
                    default:
                        return "未认证";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 初始化查询条件
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<StringBuilder, IList<SqlParam>> InitCondition()
        {
            StringBuilder sb = new StringBuilder();
            List<SqlParam> IList_param = new List<SqlParam>();

            // 关键字（登录名、真实姓名、手机号）
            if (!string.IsNullOrEmpty(txtUserName.Value))
            {
                sb.Append(" and (b.RealName like @name or b.BindPhone like @name) ");
                IList_param.Add(new SqlParam("@name", '%' + txtUserName.Value.Trim() + '%'));
            }
            // 订单号
            if (!string.IsNullOrEmpty(txtOrdersNo.Value))
            {
                sb.Append(" and (a.OrderNum like @OrdersNo or b.OutOrderNo like @OrdersNo)");
                IList_param.Add(new SqlParam("@OrdersNo", '%' + txtOrdersNo.Value.Trim()));
            }
            // 车辆名称
            if (!string.IsNullOrEmpty(txtVehicleName.Value))
            {
                sb.Append(" and c.LicenseNumber like @LicenseNumber ");
                IList_param.Add(new SqlParam("@LicenseNumber", '%' + txtVehicleName.Value.Trim() + '%'));
            }
            // 订单状态
            if (selOrderState.Value != "-1")
            {
                sb.Append(" and a.OrderState=@OrderState ");
                IList_param.Add(new SqlParam("@OrderState", selOrderState.Value));
            }
            // 订单起始生成时间
            if (!string.IsNullOrEmpty(txtStartCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCreateTime,a.CreateTime)>=0");
                IList_param.Add(new SqlParam("@StartCreateTime", txtStartCreateTime.Value));
            }
            // 订单结束生成时间
            if (!string.IsNullOrEmpty(txtEndCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,a.CreateTime,@EndCreateTime)>=0 ");
                IList_param.Add(new SqlParam("@EndCreateTime", txtEndCreateTime.Value));
            }
            // 订单起始完成时间
            if (!string.IsNullOrEmpty(txtStartFinishedTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartFinishedTime,a.FinishedTime)>=0");
                IList_param.Add(new SqlParam("@StartFinishedTime", txtStartFinishedTime.Value));
            }
            // 订单结束完成时间
            if (!string.IsNullOrEmpty(txtEndFinishedTime.Value))
            {
                sb.Append(" and DATEDIFF(d,a.FinishedTime,@EndFinishedTime)>=0 ");
                IList_param.Add(new SqlParam("@EndFinishedTime", txtEndFinishedTime.Value));
            }
            // 订单来源
            if (selOrderSource.Value != "-1")
            {
                sb.Append(" and a.OrderSource=@OrderSource ");
                IList_param.Add(new SqlParam("@OrderSource", selOrderSource.Value));
            }
            
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void ClearInput()
        {
            txtOrdersNo.Value = "";
            txtUserName.Value = "";
            txtVehicleName.Value = "";
            selOrderState.Value = "-1";
            txtStartCreateTime.Value = "";
            txtStartFinishedTime.Value = "";
            //txtStartMileage.Value = "";
            //txtStartMinutes.Value = "";
            txtEndCreateTime.Value = "";
            txtEndFinishedTime.Value = "";
            //txtEndMileage.Value = "";
            //txtEndMinutes.Value = "";
            selOrderSource.Value = "-1";
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
        }

        protected void lbtExport_Click(object sender, EventArgs e)
        {
            OrdersManager ordersManager = new OrdersManager();
            int count =0;
            int pageIndex =1;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = ordersManager.GetOrdersInfoPage(keyValue.Key, keyValue.Value, pageIndex, 1000000000, ref count);

            StringBuilder s=new StringBuilder();
            s.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            s.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");

            //写出列名
            s.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
            s.AppendLine("<td>订单编号</td><td>第三方订单号</td><td>会员手机号</td><td>会员姓名</td><td>车辆</td><td>公里价格</td><td>分钟价格</td><td>公里数</td><td>分钟数</td><td>总金额</td><td>结算金额</td><td>支付金额</td><td>订单状态</td><td>支付状态</td><td>开始时间</td><td>结束时间</td>");
            s.AppendLine("</tr>");

            //写数据
            foreach (DataRow row in dt.Rows)
            {
                s.Append("<tr>");
                s.Append("<td>~").Append(row["OrderNum"].ToString()).Append("</td>");
                s.Append("<td>~").Append(row["OutOrderNo"].ToString()).Append("</td>");
                s.Append("<td>~").Append(row["BindPhone"].ToString()).Append("</td>");
                if (!string.IsNullOrEmpty(row["RealName"].ToString()))
                {
                    s.Append("<td>").Append(row["RealName"].ToString()).Append("</td>");
                }
                else 
                {
                    s.Append("<td>").Append("匿名").Append("</td>");
                }
                s.Append("<td>~").Append(row["LicenseNumber"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["KMPrice"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["MinutePrice"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["Mileage"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["Minutes"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["TotalMoney"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["SettlementMoney"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["PayMoney"].ToString()).Append("</td>");
                s.Append("<td>").Append(EnumHelper.GetEnumShowName(typeof(OrderState), Convert.ToInt32(row["OrderState"].ToString()))).Append("</td>");
                s.Append("<td>").Append(EnumHelper.GetEnumShowName(typeof(OrderPayState), Convert.ToInt32(row["PayState"].ToString()))).Append("</td>");
                s.Append("<td>").Append(row["CreateTime"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["FinishedTime"].ToString()).Append("</td>");                
                s.AppendLine("</tr>");
            }
            s.AppendLine("</table>");
            this.Page.Response.ContentType = "application/vnd.ms-excel";
            this.Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
            this.Page.Response.Charset = "Utf-8";
            this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=订单列表"+DateTime.Now.ToString("yyyyMMddHHmmss")+".xls");
            this.Page.Response.Write(s.ToString());
            this.Page.Response.End();
            /*
            s.Append("订单编号,会员手机号,会员姓名,车辆,公里价格,分钟价格,公里数,分钟数,总金额,结算金额,订单状态,支付状态,开始时间,结束时间\r\n");
            foreach (DataRow row in dt.Rows)
            {
                s.Append(row["OrderNum"].ToString());
                s.Append(",").Append(row["BindPhone"].ToString());
                s.Append(",").Append(row["RealName"].ToString());
                s.Append(",").Append(row["VehicleName"].ToString());
                s.Append(",").Append(row["KMPrice"].ToString());
                s.Append(",").Append(row["MinutesPrice"].ToString());
                s.Append(",").Append(row["Mileage"].ToString());
                s.Append(",").Append(row["Minutes"].ToString());
                s.Append(",").Append(row["TotalMoney"].ToString());
                s.Append(",").Append(row["SettlementMoney"].ToString());
                s.Append(",").Append(EnumHelper.GetEnumShowName(typeof(OrderState), Convert.ToInt32(row["OrderState"].ToString())));
                s.Append(",").Append(EnumHelper.GetEnumShowName(typeof(OrderPayState), Convert.ToInt32(row["PayState"].ToString())));
                s.Append(",").Append(row["CreateTime"].ToString());
                s.Append(",").Append(row["FinishedTime"].ToString()).Append("\r\n");
            }
            this.Page.Response.Clear();
            this.Page.Response.Buffer = false;
            this.Page.Response.Charset = "GB2312";
            this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=订单列表.csv");
            this.Page.Response.ContentEncoding = Encoding.GetEncoding("GB2312");
            this.Page.Response.ContentType = "application/octet-stream";
            this.Page.EnableViewState = false;
            this.Page.Response.Write(s);
            this.Page.Response.End();
             */ 
        }
    }
}