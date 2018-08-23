using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using System.Xml;
using YR.Common.DotNetUI;
using YR.Common.DotNetConfig;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetCode;
using System.Data;


namespace YR.Web.YRBase.SysConfig
{
    public partial class DictList : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                DictManager dicManager = new DictManager();
                DataTable dtDic = dicManager.GetDict();
                if (dtDic.Rows.Count > 0)
                {

                    ////网点马厩绑定
                    drpDic.DataSource = dtDic;
                    drpDic.DataTextField = "Name";
                    drpDic.DataValueField = "ID";
                    drpDic.DataBind();
                    drpDic.Items.Insert(0, new ListItem("--------请选择--------", "-1"));

                }
                DataBindGrid();
            }

        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid()
        {

            DictManager dicManager = new DictManager();

          


            int count = 0;
            StringBuilder SqlWhere = new StringBuilder();
            IList<SqlParam> IList_param = new List<SqlParam>();
            if (!this.drpDic.Value.ToString().Equals("-1"))
            {
                SqlWhere.Append(" and DictID = @obj ");
                IList_param.Add(new SqlParam("@obj", this.drpDic.Value.ToString()));
            }

            DataTable dt = dicManager.GetPitsInfoPage(SqlWhere, IList_param, PageControl1.PageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.RecordCount = Convert.ToInt32(count);
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

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }
    }
}