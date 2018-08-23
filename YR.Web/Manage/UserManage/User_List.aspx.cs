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
    public partial class User_List : PageBase
    {
        public StringBuilder str_tableTree = new StringBuilder();

        YR_UserInfo_IDAO user_idao = new YR_UserInfo_Dal();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                BindUserType();
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

        private void BindUserType()
        {
            UserInfoManager userManager = new UserInfoManager();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataRow newRow = dt.NewRow();
            newRow["ID"] = "-1";
            newRow["Name"] = "全部";
            dt.Rows.Add(newRow);
            DataTable usertype_dt = userManager.GetUserTypeList();
            foreach (DataRow row in usertype_dt.Rows)
            {
                newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            selUserType.DataSource = dt;
            selUserType.DataTextField = "Name";
            selUserType.DataValueField = "ID";
            selUserType.DataBind();
        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            UserInfoManager userManager = new UserInfoManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = userManager.GetUserInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
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
                sb.Append(" and (UserName like @name or RealName like @name or BindPhone like @name) ");
                IList_param.Add(new SqlParam("@name", '%' + txt_Search.Value.Trim() + '%'));
            }
            // 实名认证
            if (selRealNameCertification.Value != "-1")
            {
                sb.Append(" and RealNameCertification=@RealNameCertification ");
                IList_param.Add(new SqlParam("@RealNameCertification", selRealNameCertification.Value));
            }
            // 用户类型
            if (selUserType.Value != "-1")
            {
                sb.Append(" and UserType=@UserType ");
                IList_param.Add(new SqlParam("@UserType", selUserType.Value));
            }
            // 账户起始余额
            if (!string.IsNullOrEmpty(start_money.Value))
            {
                sb.Append(" and Balance>=@StartBalance ");
                IList_param.Add(new SqlParam("@StartBalance", start_money.Value));
            }
            // 账户结束余额
            if (!string.IsNullOrEmpty(end_money.Value))
            {
                sb.Append(" and Balance<=@EndBalance ");
                IList_param.Add(new SqlParam("@EndBalance", end_money.Value));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
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
        /// 清空输入框
        /// </summary>
        private void ClearInput()
        {
            txt_Search.Value = "";
            selRealNameCertification.Value = "-1";
            start_money.Value = "";
            end_money.Value = "";
            selUserType.Value = "-1";
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
            UserInfoManager userManager = new UserInfoManager();
            int count = 0;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = userManager.GetUserInfoPage(keyValue.Key, keyValue.Value, 1, int.MaxValue, ref count);

            StringBuilder s = new StringBuilder();
            s.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            s.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");

            //写出列名
            s.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
            s.AppendLine("<td>绑定手机</td><td>真实姓名</td><td>昵称</td><td>是否实名认证</td><td>账户余额</td><td>副账户余额</td><td>押金金额</td><td>用户类型</td><td>状态</td><td>创建时间</td>");
            s.AppendLine("</tr>");

            //写数据
            foreach (DataRow row in dt.Rows)
            {
                s.Append("<tr>");
                s.Append("<td>").Append(row["BindPhone"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["RealName"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["NickName"].ToString()).Append("</td>");
                s.Append("<td>").Append(EnumHelper.GetEnumShowName(typeof(UserRealNameCertification), Convert.ToInt32(row["RealNameCertification"].ToString()))).Append("</td>");
                s.Append("<td>").Append(row["Balance"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["Balance2"].ToString()).Append("</td>");
                s.Append("<td>").Append(row["Deposit"].ToString()).Append("</td>");
                s.Append("<td>").Append(EnumHelper.GetEnumShowName(typeof(UserType), Convert.ToInt32(row["UserType"].ToString()))).Append("</td>");
                s.Append("<td>").Append(row["UserState"].ToString() == "0" ? "禁用" : "启用").Append("</td>");
                s.Append("<td>").Append(row["RegistrionTime"].ToString()).Append("</td>");
                s.AppendLine("</tr>");
            }
            s.AppendLine("</table>");
            this.Page.Response.ContentType = "application/vnd.ms-excel";
            this.Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
            this.Page.Response.Charset = "Utf-8";
            this.Page.Response.AppendHeader("Content-Disposition", "attachment;filename=用户列表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            this.Page.Response.Write(s.ToString());
            this.Page.Response.End();
        }

    }
}