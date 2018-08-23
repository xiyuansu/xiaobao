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
    public partial class VehicleCommentsLog_List : PageBase
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
        private void DataBindGrid(bool isQuery = false)
        {
            VehicleCommentsLogManager vehicleCommentsLogManager = new VehicleCommentsLogManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();

            DataTable dt = vehicleCommentsLogManager.GetVehicleCommentsLogInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);

            this.PageControl1.PageIndex = pageIndex;
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

            // 关键字（登录名、真实姓名、手机号）
            if (!string.IsNullOrEmpty(txt_Search.Value))
            {
                sb.Append(" and (c.NickName like @name or c.RealName like @name or c.BindPhone like @name) ");
                IList_param.Add(new SqlParam("@name", '%' + txt_Search.Value.Trim() + '%'));
            }
            // 车辆名称
            if (!string.IsNullOrEmpty(txtVehicleName.Value))
            {
                sb.Append(" and b.LicenseNumber like @LicenseNumber ");
                IList_param.Add(new SqlParam("@LicenseNumber", '%' + txtVehicleName.Value.Trim() + '%'));
            }
            // 状态
            if (selState.Value!="-1")
            {
                sb.Append(" and a.[State] = @State ");
                IList_param.Add(new SqlParam("@State", selState.Value));
            }
            // 客户端
            if (selClientType.Value != "-1")
            {
                sb.Append(" and a.ClientType = @ClientType ");
                IList_param.Add(new SqlParam("@ClientType", selClientType.Value));
            }
            // 评论起始时间
            if (!string.IsNullOrEmpty(txtStartCommentTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCommentTime,a.CommentsTime)>=0");
                IList_param.Add(new SqlParam("@StartCommentTime", txtStartCommentTime.Value));
            }
            // 评论结束时间
            if (!string.IsNullOrEmpty(txtEndCommentTime.Value))
            {
                sb.Append(" and DATEDIFF(d,a.CommentsTime,@EndCommentTime)>=0 ");
                IList_param.Add(new SqlParam("@EndCommentTime", txtEndCommentTime.Value));
            }

            sb.Append(" and len(a.CommentsContent)>@ContentLength ");
            IList_param.Add(new SqlParam("@ContentLength", 0));

            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void ClearInput()
        {
            txt_Search.Value = "";
            txtVehicleName.Value = "";
            txtStartCommentTime.Value = "";
            txtEndCommentTime.Value = "";
            selState.Value = "-1";
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
            VehicleCommentsLogManager vehicleCommentsLogManager = new VehicleCommentsLogManager();
            int count = 0;
            int pageIndex = 1;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = vehicleCommentsLogManager.GetVehicleCommentsLogInfoPage(keyValue.Key, keyValue.Value, pageIndex, 1000000000, ref count);
            
            StringBuilder s = new StringBuilder();
            s.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            s.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");

            //写出列名
            s.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
            s.AppendLine("<td>车辆名称</td><td>会员手机号</td><td>会员姓名</td><td>会员昵称</td><td>评论内容</td><td>评论时间</td><td>评论状态</td><td>客户端</td>");
            s.AppendLine("</tr>");

            //写数据
            foreach (DataRow row in dt.Rows)
            {
                s.Append("<tr>");
                s.Append("<td>'").Append(row["LicenseNumber"].ToString()).Append("</td>");
                s.Append("<td>'").Append(row["BindPhone"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["RealName"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["UserNickName"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["CommentsContent"].ToString()).Append("</td>"); 
                s.Append("<td>'").Append(row["CommentsTime"].ToString()).Append("</td>");
                s.Append("<td>'").Append(EnumHelper.GetEnumShowName(typeof(UserRaiseReplyState), Convert.ToInt32(row["State"].ToString()))).Append("</td>");
                s.Append("<td>").Append(row["ClientType"].ToString()).Append("</td>");
                s.AppendLine("</tr>");
            }
            s.AppendLine("</table>");
            this.Page.Response.ContentType = "application/vnd.ms-excel";
            this.Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
            this.Page.Response.Charset = "Utf-8";
            this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=评价列表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            this.Page.Response.Write(s.ToString());
            this.Page.Response.End();
        }
    }
}