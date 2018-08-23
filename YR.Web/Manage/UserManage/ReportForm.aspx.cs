using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using YR.Common.DotNetUI;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.UserManage
{
    public partial class ReportForm : PageBase
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
            string startDate = txtStartDate.Value+" 00:00:00";
            string endDate = txtEndDate.Value+" 23:59:59";
            OrdersManager ordersManager = new OrdersManager();
            dt= ordersManager.GetOrdersReport(startDate, endDate);
            if (dt != null)
            {
                dt.Columns.Add("ConversionRate", Type.GetType("System.String"));
                dt.Columns.Add("ReservationRate", Type.GetType("System.String"));
                dt.Columns.Add("SubscriptionRate", Type.GetType("System.String")); 
                dt.Columns.Add("SubscriptionVehicleRatio", Type.GetType("System.String"));
                dt.Columns.Add("StartRate", Type.GetType("System.String"));
                dt.Columns.Add("DrivingRate", Type.GetType("System.String"));
                dt.Columns.Add("ConsentRate", Type.GetType("System.String"));
                dt.Columns.Add("FailureRate", Type.GetType("System.String"));
                dt.Columns.Add("PaymentRate", Type.GetType("System.String"));
                dt.Columns.Add("ViolationRate", Type.GetType("System.String"));
                dt.Columns.Add("RefundRate", Type.GetType("System.String"));
                dt.Columns.Add("ReturnRate", Type.GetType("System.String"));
                dt.Columns.Add("AverageDailyOrder", Type.GetType("System.String"));
                dt.Columns.Add("AverageDailyIncome", Type.GetType("System.String"));

                foreach (DataRow dr in dt.Rows)
                {
                    decimal orderCount = decimal.Parse(dr["OrderCount"].ToString());
                    decimal paymentCount = decimal.Parse(dr["PaymentCount"].ToString());
                    decimal vehicleCount = decimal.Parse(dr["VehicleCount"].ToString());
                    decimal startCount = decimal.Parse(dr["StartCount"].ToString());
                    decimal endCount = 0.00m;
                    decimal.TryParse(dr["EndCount"].ToString(),out endCount);
                    decimal normalCount = 0.00m;
                    decimal.TryParse(dr["NormalCount"].ToString(),out normalCount);
                    decimal abnormalCount = 0.00m;
                    decimal.TryParse(dr["AbnormalCount"].ToString(),out abnormalCount);
                    decimal paidCount = 0.00m;
                    decimal.TryParse(dr["PaidCount"].ToString(),out paidCount);
                    decimal cancelCount = 0.00m;
                    decimal.TryParse(dr["CancelCount"].ToString(),out cancelCount);
                    decimal income = 0.00m;
                    decimal.TryParse(dr["Income"].ToString(),out income);
                    decimal payNormalVehicleCount = 0.00m;
                    decimal.TryParse(dr["PayNormalVehicleCount"].ToString(),out paymentCount);
                    
                    //转化率=付款单数/意向单数
                    dr["ConversionRate"] = "0";
                    if (paymentCount > 0 && orderCount > 0)
                    {
                        dr["ConversionRate"] = String.Format("{0:F}", (paymentCount / orderCount) * 100) + "%";
                    }
                    //预订率 = 预定次数 / 意向单数
                    dr["ReservationRate"] = "100%";//String.Format("{0:F}", (orderCount / orderCount) * 100) + "%";
                    //付订率=付订单数/预定次数
                    dr["SubscriptionRate"] = "100%";//String.Format("{0:F}", (orderCount/orderCount) * 100) + "%";
                    //付订车数比=付订单数/付订车数
                    dr["SubscriptionVehicleRatio"] = "0";
                    if (orderCount > 0 && vehicleCount > 0)
                    {
                        String.Format("{0:F}", (orderCount / vehicleCount));
                    }
                    //取车率=取车单数/付订单数
                    dr["StartRate"] = "0";
                    if (startCount > 0 && orderCount > 0)
                    {
                        dr["StartRate"] = String.Format("{0:F}", (startCount / orderCount) * 100) + "%";
                    }
                    //行驶率=行驶大于200米的订单数/取车单数
                    dr["DrivingRate"] ="0";
                    if (normalCount > 0 && orderCount > 0)
                    {
                        dr["DrivingRate"] = String.Format("{0:F}", (normalCount / orderCount) * 100) + "%";
                    }
                    //同意单数	同意率=同意单数/付订单数	
                    dr["ConsentRate"] = "100%";
                                               //失败单数  失败率 = 失败单数 / 付款单数
                    dr["FailureRate"] = "0";
                    if (abnormalCount > 0)
                    {
                        dr["FailureRate"] = String.Format("{0:F}", (abnormalCount / orderCount) * 100) + "%";
                    }
                    //付款率 = 付款单数 / 取车单数
                    dr["PaymentRate"] = "0";
                    if (paidCount > 0 && startCount > 0)
                    {
                        dr["PaymentRate"] = String.Format("{0:F}", (paidCount / startCount) * 100) + "%";
                    }
                    //违押单数 违押率 = 违押单数 / 付款单数
                    dr["ViolationRate"] = "100%";//String.Format("{0:F}", (orderCount / orderCount) * 100) + "%";
                                                 //退款率 = 退款单数 / 付款单数
                    dr["RefundRate"] = "0";
                    if (cancelCount > 0 && paymentCount > 0)
                    {
                        dr["RefundRate"] = String.Format("{0:F}", (cancelCount / paymentCount) * 100) + "%";
                    }
                    //还车单率 = (操作还车单数 - 取消单数) / 付款单数
                    dr["ReturnRate"] = "0";
                    if (paymentCount > 0)
                    {
                        dr["ReturnRate"] = String.Format("{0:F}", ((paymentCount - cancelCount) / paymentCount) * 100) + "%";
                    }
                    //日车均订单数 = 行驶大于200米的订单数/ 有出租记录的车辆
                    dr["AverageDailyOrder"] = "0";
                    if (normalCount > 0 && vehicleCount > 0)
                    {
                        dr["AverageDailyOrder"] = String.Format("{0:F}", (normalCount / vehicleCount));
                    }
                    //日车均收入(出租) = 当天收入 / 产生收入的车辆数
                    dr["AverageDailyIncome"] = "0";
                    if (income > 0 && payNormalVehicleCount > 0)
                    {
                        dr["AverageDailyIncome"] = String.Format("{0:F}", (income / payNormalVehicleCount));
                    }
                }
            }
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
            if (!string.IsNullOrEmpty(startDate) &&!string.IsNullOrEmpty(endDate))
            {
                DateTime dtStart = Convert.ToDateTime(startDate);
                DateTime dtEnd = Convert.ToDateTime(endDate);
                if (DateTime.Compare(dtEnd,dtStart) > 0)
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
            dt = ordersManager.GetOrdersReport(startDate, endDate);
            if (dt != null)
            {
                dt.Columns.Add("ConversionRate", Type.GetType("System.String"));
                dt.Columns.Add("ReservationRate", Type.GetType("System.String"));
                dt.Columns.Add("SubscriptionRate", Type.GetType("System.String"));
                dt.Columns.Add("SubscriptionVehicleRatio", Type.GetType("System.String"));
                dt.Columns.Add("StartRate", Type.GetType("System.String"));
                dt.Columns.Add("DrivingRate", Type.GetType("System.String"));
                dt.Columns.Add("ConsentRate", Type.GetType("System.String"));
                dt.Columns.Add("FailureRate", Type.GetType("System.String"));
                dt.Columns.Add("PaymentRate", Type.GetType("System.String"));
                dt.Columns.Add("ViolationRate", Type.GetType("System.String"));
                dt.Columns.Add("RefundRate", Type.GetType("System.String"));
                dt.Columns.Add("ReturnRate", Type.GetType("System.String"));
                dt.Columns.Add("AverageDailyOrder", Type.GetType("System.String"));
                dt.Columns.Add("AverageDailyIncome", Type.GetType("System.String"));

                foreach (DataRow dr in dt.Rows)
                {
                    decimal orderCount = decimal.Parse(dr["OrderCount"].ToString());
                    decimal paymentCount = decimal.Parse(dr["PaymentCount"].ToString());
                    decimal vehicleCount = decimal.Parse(dr["VehicleCount"].ToString());
                    decimal startCount = decimal.Parse(dr["StartCount"].ToString());
                    decimal endCount = decimal.Parse(dr["EndCount"].ToString());
                    decimal normalCount = decimal.Parse(dr["NormalCount"].ToString());
                    decimal abnormalCount = decimal.Parse(dr["AbnormalCount"].ToString());
                    decimal paidCount = decimal.Parse(dr["PaidCount"].ToString());
                    decimal cancelCount = decimal.Parse(dr["CancelCount"].ToString());
                    decimal income = decimal.Parse(dr["Income"].ToString());
                    decimal payNormalVehicleCount = decimal.Parse(dr["PayNormalVehicleCount"].ToString());

                    //转化率=付款单数/意向单数
                    dr["ConversionRate"] = "0";
                    if (paymentCount > 0 && orderCount > 0)
                    {
                        dr["ConversionRate"] = String.Format("{0:F}", (paymentCount / orderCount) * 100) + "%";
                    }
                    //预订率 = 预定次数 / 意向单数
                    dr["ReservationRate"] = "100%";//String.Format("{0:F}", (orderCount / orderCount) * 100) + "%";
                    //付订率=付订单数/预定次数
                    dr["SubscriptionRate"] = "100%";//String.Format("{0:F}", (orderCount/orderCount) * 100) + "%";
                    //付订车数比=付订单数/付订车数
                    dr["SubscriptionVehicleRatio"] = String.Format("{0:F}", (orderCount / vehicleCount));
                    //取车率=取车单数/付订单数
                    dr["StartRate"] = String.Format("{0:F}", (startCount / orderCount) * 100) + "%";
                    //行驶率=行驶大于200米的订单数/取车单数
                    dr["DrivingRate"] = String.Format("{0:F}", (normalCount / orderCount) * 100) + "%";
                    //同意单数	同意率=同意单数/付订单数	
                    dr["ConsentRate"] = "100%";
                    //失败单数  失败率 = 失败单数 / 付款单数
                    dr["FailureRate"] = "0";
                    if (abnormalCount > 0)
                    {
                        dr["FailureRate"] = String.Format("{0:F}", (abnormalCount / orderCount) * 100) + "%";
                    }
                    //付款率 = 付款单数 / 取车单数
                    dr["PaymentRate"] = "0";
                    if (paidCount > 0 && startCount > 0)
                    {
                        dr["PaymentRate"] = String.Format("{0:F}", (paidCount / startCount) * 100) + "%";
                    }
                    //违押单数 违押率 = 违押单数 / 付款单数
                    dr["ViolationRate"] = "100%";//String.Format("{0:F}", (orderCount / orderCount) * 100) + "%";
                                                 //退款率 = 退款单数 / 付款单数
                    dr["RefundRate"] = "0";
                    if (cancelCount > 0 && paymentCount > 0)
                    {
                        dr["RefundRate"] = String.Format("{0:F}", (cancelCount / paymentCount) * 100) + "%";
                    }
                    //还车单率 = (还车单数 - 取消单数) / 付款单数
                    dr["ReturnRate"] = "0";
                    if (paymentCount > 0)
                    {
                        dr["ReturnRate"] = String.Format("{0:F}", ((paymentCount - cancelCount) / paymentCount) * 100) + "%";
                    }
                    //日车均订单数 = 行驶大于200米的订单数/ 有出租记录的车辆
                    dr["AverageDailyOrder"] = "0";
                    if (normalCount > 0 && vehicleCount > 0)
                    {
                        dr["AverageDailyOrder"] = String.Format("{0:F}", (normalCount / vehicleCount));
                    }
                    //日车均收入(出租) = 当天收入 / 产生收入的车辆数
                    dr["AverageDailyIncome"] = "0";
                    if (income > 0 && payNormalVehicleCount > 0)
                    {
                        dr["AverageDailyIncome"] = String.Format("{0:F}", (income / payNormalVehicleCount));
                    }
                }

            }
            if (dt != null&&dt.Rows.Count>0)
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                s.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
                //写出列名
                s.AppendLine("<tr>");
                s.AppendLine("<td>日期</td><td>意向单数</td><td>转化率</td><td>预订次数</td><td>预订率</td><td>付订单数</td><td>付订率</td><td>付订车数</td>" +
                    "<td>付订车数比</td><td>取车单数</td><td>取车率</td><td>行驶单数</td><td>行驶率</td><td>同意单数</td><td>同意率</td>" +
                    "<td>失败单数</td><td>失败率</td><td>付款单数</td><td>付款率</td><td>违押单数</td><td>违押率</td>" +
                    "<td>退款单数</td><td>退款率</td><td>日车均订单数</td><td>日车均收入(出租)</td><td>还车单数</td><td>还车单率</td>");
                s.AppendLine("</tr>");

                //写数据
                foreach (DataRow row in dt.Rows)
                {
                    s.Append("<tr>");
                    s.Append("<td>").Append(row["ReportTime"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["OrderCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["ConversionRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["OrderCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["ReservationRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["OrderCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["SubscriptionRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["VehicleCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["SubscriptionVehicleRatio"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["StartCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["StartRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["NormalCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["DrivingRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["OrderCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["ConsentRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["AbnormalCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["FailureRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["PaymentCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["PaymentRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["OrderCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["ViolationRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["CancelCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["RefundRate"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["AverageDailyOrder"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["AverageDailyIncome"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["EndCount"].ToString()).Append("</td>");
                    s.Append("<td>").Append(row["ReturnRate"].ToString()).Append("</td>");
                    s.AppendLine("</tr>");
                }
                s.AppendLine("</table>");
                this.Page.Response.ContentType = "application/vnd.ms-excel";
                this.Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
                this.Page.Response.Charset = "Utf-8";
                this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=小宝出行报表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                this.Page.Response.Write(s.ToString());
                this.Page.Response.End();
            }
        }
    }
}