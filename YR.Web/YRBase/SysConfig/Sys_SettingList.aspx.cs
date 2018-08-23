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

namespace YR.Web.YRBase.SysConfig
{
    public partial class Sys_SettingList : PageBase
    {
        private SysSettingManager settingManager = new SysSettingManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                DataBinds();
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
            int count = 0;
            StringBuilder SqlWhere = new StringBuilder();
            IList<SqlParam> IList_param = new List<SqlParam>();
            if (!string.IsNullOrEmpty(txtName.Value))
            {
                SqlWhere.Append(" and s.Name like @name ");
                IList_param.Add(new SqlParam("@name", '%' + txtName.Value.Trim() + '%'));

            }

            if (!this.drpDic.Value.ToString().Equals("-1"))
            {
                SqlWhere.Append(" and SettingType = @dictId ");
                IList_param.Add(new SqlParam("@dictId", this.drpDic.Value.ToString()));
            }



            DataTable dt = settingManager.GetDataInfoPage(SqlWhere, IList_param, PageControl1.PageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.RecordCount = Convert.ToInt32(count);
        }


        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        public void DataBinds()
        {
            DataTable dtDic = settingManager.GetBaseDictList();
            if (dtDic.Rows.Count > 0)
            {
                ////网点马厩绑定
                drpDic.DataSource = dtDic;
                drpDic.DataTextField = "Name";
                drpDic.DataValueField = "ID";
                drpDic.DataBind();
                drpDic.Items.Insert(0, new ListItem("--------请选择--------", "-1"));

            }
        }

    }
}