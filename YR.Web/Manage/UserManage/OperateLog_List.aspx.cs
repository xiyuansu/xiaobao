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
    public partial class OperateLog_List : System.Web.UI.Page
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
            OPUserManager userManager = new OPUserManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = userManager.GetOperateLogPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
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

        }

        /// <summary>
        /// 初始化查询条件
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<StringBuilder, IList<SqlParam>> InitCondition()
        {
            StringBuilder sb = new StringBuilder();
            List<SqlParam> IList_param = new List<SqlParam>();
            if (!string.IsNullOrEmpty(txtUserName.Value))
            {
                sb.Append(" and (u.UserName like @UserName or u.NickName like @UserName or u.Tel like @UserName)");
                IList_param.Add(new SqlParam("@UserName", '%' + txtUserName.Value.Trim() + '%'));
            }

            // 车辆名称
            if (!string.IsNullOrEmpty(txtVehicleName.Value))
            {
                sb.Append(" and v.VehicleName like @VehicleName ");
                IList_param.Add(new SqlParam("@VehicleName", '%' + txtVehicleName.Value.Trim() + '%'));
            }
            // 操作类型
            if (selOperateType.Value != "-1")
            {
                sb.Append(" and OperateType=@OperateType ");
                IList_param.Add(new SqlParam("@OperateType", selOperateType.Value));
            }
            // 起始时间
            if (!string.IsNullOrEmpty(txtStartOperateTime.Value))
            {
                sb.Append(" and l.OperateTime>=@StartOperateTime ");
                IList_param.Add(new SqlParam("@StartOperateTime", txtStartOperateTime.Value + " 00:00:00"));
            }
            // 结束时间
            if (!string.IsNullOrEmpty(txtEndOperateTime.Value))
            {
                sb.Append(" and l.OperateTime<=@EndOperateTime ");
                IList_param.Add(new SqlParam("@EndOperateTime", txtEndOperateTime.Value + " 23:59:59"));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txtUserName.Value = "";
            txtVehicleName.Value = "";
            txtStartOperateTime.Value = "";
            txtEndOperateTime.Value = "";
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
    }
}