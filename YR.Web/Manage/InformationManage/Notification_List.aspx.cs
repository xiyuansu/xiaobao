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

namespace YR.Web.Manage.InformationManage
{
    public partial class Notification_List : PageBase
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
            MessageManager messageManager = new MessageManager();
            int count = 0;
            StringBuilder SqlWhere = new StringBuilder();
            IList<SqlParam> IList_param = new List<SqlParam>();
            if (!string.IsNullOrEmpty(this.txtMessageTitle.Value))
            {
                SqlWhere.Append(" and MessageTitle like @MessageTitle ");
                IList_param.Add(new SqlParam("@MessageTitle", '%' + this.txtMessageTitle.Value + '%'));
            }

            if (!this.drpState.SelectedValue.Equals("-1"))
            {
                SqlWhere.Append(" and State = '" + this.drpState.SelectedValue + "' ");
            }


            if (!string.IsNullOrEmpty(this.txtReleaseTime.Value))
            {
                SqlWhere.Append(" and ('" + this.txtReleaseTime.Value + "'>=ReleaseTime )");
            }


            if ((!string.IsNullOrEmpty(this.txtStartCreateTime.Value)) && string.IsNullOrEmpty(this.txtEndCreateTime.Value))
            {
                SqlWhere.Append(" and  '" + txtStartCreateTime.Value + "' <= EndTime");
            }
            else if (string.IsNullOrEmpty(this.txtStartCreateTime.Value) && (!string.IsNullOrEmpty(this.txtEndCreateTime.Value)))
            {
                SqlWhere.Append(" and  '" + this.txtEndCreateTime.Value + "' >= BeginTime");

            }
            else if ((!string.IsNullOrEmpty(this.txtStartCreateTime.Value)) && string.IsNullOrEmpty(this.txtEndCreateTime.Value))
            {
                SqlWhere.Append(" and  (（and  '" + this.txtStartCreateTime.Value + "' <= BeginTime and '" + this.txtEndCreateTime.Value + "'>=BeginTime)||（and  '" + this.txtStartCreateTime.Value + "' >= BeginTime and '" + this.txtStartCreateTime.Value + "'<=EndTime))");
            }
            DataTable dt = messageManager.GetPitsInfoPage(SqlWhere, IList_param, PageControl1.PageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.RecordCount = Convert.ToInt32(count);
        }

        /// <summary>
        /// 绑定后激发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
        }

        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            this.DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }
        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            this.txtMessageTitle.Value = "";
            this.txtReleaseTime.Value = "";
            this.txtStartCreateTime.Value = "";
            this.txtEndCreateTime.Value = "";
            drpState.SelectedIndex = 0;
        }
    }
}