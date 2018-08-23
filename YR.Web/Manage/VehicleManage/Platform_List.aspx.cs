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
    public partial class Platform_List : PageBase
    {
        public StringBuilder str_tableTree = new StringBuilder();
        YR_UserInfo_IDAO user_idao = new YR_UserInfo_Dal();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
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
            PlatformManager platformManager = new PlatformManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = platformManager.GetPlatformInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
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
            // 平台名称
            if (!string.IsNullOrEmpty(txtPlatform.Value))
            {
                sb.Append(" and Platform like @Platform ");
                IList_param.Add(new SqlParam("@Platform", '%' + txtPlatform.Value.Trim() + '%'));
            }
            // 平台协议代码
            if (!string.IsNullOrEmpty(txtCode.Value))
            {
                sb.Append(" and Code like @Code ");
                IList_param.Add(new SqlParam("@Code", '%' + txtCode.Value.Trim() + '%'));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txtPlatform.Value = "";
            txtCode.Value = "";
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

        protected void btnRefresh_ServerClick(object sender, EventArgs e)
        {
            this.DataBindGrid();
        }
    }
}