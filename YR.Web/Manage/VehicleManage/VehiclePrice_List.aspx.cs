using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Busines.DAL;
using YR.Busines.IDAO;
using YR.Common.DotNetBean;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehiclePrice_List : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                BindCity();
                BindModel();
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
            VehiclePriceManager priceManager = new VehiclePriceManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;

            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt =priceManager.GetPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.PageIndex = pageIndex;
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
        }

        private void BindCity()
        {
            DictManager dictManager = new DictManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataRow newRow = dt.NewRow();
            newRow["ID"] = "-1";
            newRow["Name"] = "全部城市";
            dt.Rows.Add(newRow);
            DataTable platformTable = dictManager.GetDictList("03");
            foreach (DataRow row in platformTable.Rows)
            {
                newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            selCity.DataSource = dt;
            selCity.DataTextField = "Name";
            selCity.DataValueField = "ID";
            selCity.DataBind();
        }

        private void BindModel()
        {
            VehicleModelManager modelManager = new VehicleModelManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataRow newRow = dt.NewRow();
            newRow["ID"] = "-1";
            newRow["Name"] = "全部车型";
            dt.Rows.Add(newRow);
            DataTable platformTable = modelManager.GetAllModelInfo();
            foreach (DataRow row in platformTable.Rows)
            {
                newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            selModel.DataSource = dt;
            selModel.DataTextField = "Name";
            selModel.DataValueField = "ID";
            selModel.DataBind();
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
            if (selModel.Value!="-1")
            {
                sb.Append(" and ModelID=@ModelID ");
                IList_param.Add(new SqlParam("@ModelID", selModel.Value));
            }
            if (selCity.Value != "-1")
            {
                sb.Append(" and CityID=@CityID ");
                IList_param.Add(new SqlParam("@CityID", selCity.Value));
            }

            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            selModel.SelectedIndex = 0;
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