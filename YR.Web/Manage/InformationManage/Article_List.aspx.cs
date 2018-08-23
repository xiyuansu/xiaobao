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

namespace YR.Web.Manage.InfomationManage
{
    public partial class Article_List : PageBase
    {
        private ArticleManager articleManager = new ArticleManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                BindCatelog();
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

        private void BindCatelog()
        {
            DictManager dictManager = new DictManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataRow defaultRow = dt.NewRow();
            defaultRow["ID"] = "";
            defaultRow["Name"] = "全部分类";
            dt.Rows.Add(defaultRow);
            DataTable dt2 = dictManager.GetDictList("04");
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            selCatelog.DataSource = dt;
            selCatelog.DataTextField = "Name";
            selCatelog.DataValueField = "ID";
            selCatelog.DataBind();
        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid()
        {
            VehicleModelManager vModelManage = new VehicleModelManager();
            int count = 0;
            StringBuilder SqlWhere = new StringBuilder();
            IList<SqlParam> IList_param = new List<SqlParam>();
            if (!string.IsNullOrEmpty(txtTitle.Value))
            {
                SqlWhere.Append(" and ArticleName like @ArticleName ");
                IList_param.Add(new SqlParam("@ArticleName", '%' + txtTitle.Value + '%'));
            }
            if (!string.IsNullOrEmpty(selCatelog.Value))
            {
                SqlWhere.Append(" and CategoryID=@CategoryID ");
                IList_param.Add(new SqlParam("@CategoryID", selCatelog.Value));
            }

            DataTable dt =articleManager.GetDataPage(SqlWhere, IList_param, PageControl1.PageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.RecordCount = Convert.ToInt32(count);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void ClearInput()
        {
            txtTitle.Value = "";
            selCatelog.SelectedIndex = -1;
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

            }
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            DataBindGrid();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "publicobjcss();", true);
        }

        // 重置
        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
        }

        public string GetCatelog(string code)
        {
            try
            {
                DictManager dictManager = new DictManager();
                return dictManager.GetItemName(code);
            }
            catch
            {
                return "";
            }
        }
    }
}