using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using YR.Busines.IDAO;
using YR.Common.DotNetBean;
using YR.Busines;
using YR.Common.DotNetUI;
using YR.Common.DotNetData;
using YR.Common.DotNetCode;
using YR.Common.DotNetEncrypt;
using YR.Busines.DAL;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.InfomationManage
{
    public partial class Article_Form : PageBase
    {
        private string _key;

        private ArticleManager articleManager = new ArticleManager();

        private void BindCatelog()
        {
            DictManager dictManager = new DictManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataTable dt2 = dictManager.GetDictList("04");
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            CategoryID.DataSource = dt;
            CategoryID.DataTextField = "Name";
            CategoryID.DataValueField = "ID";
            CategoryID.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];                  //主键
            if (!IsPostBack)
            {
                BindCatelog();
                if (!string.IsNullOrEmpty(_key))
                {
                    InitData();
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            Hashtable ht = articleManager.GetArticleInfoByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
        }

        /// <summary>
        /// 验证是否存在
        /// </summary>
        /// <param name="pkName">对象主键</param>
        /// <param name="Obj_id">对象主键值</param>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public string GetChecked(string pkName, string Obj_Val, DataTable dt)
        {
            StringBuilder strSql = new StringBuilder();
            dt = DataTableHelper.GetNewDataTable(dt, "" + pkName + " = '" + Obj_Val + "'");
            if (DataTableHelper.IsExistRows(dt))
                return "checked=\"checked\"";
            else
                return "";
        }

        #region 保存事件
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                Hashtable ht = new Hashtable();
                ht["CategoryID"] = CategoryID.Value;
                // 文章名称
                ht["ArticleName"] = ArticleName.Value;
                // 文章内容
                ht["ArticleContent"] = ArticleContent.Value;
                ht["Sort"] = string.IsNullOrEmpty(Sort.Value) ? "9999" : Sort.Value;
                ht["ReleaseTime"] = DateTime.Now;
                ht["Releaser"] = RequestSession.GetSessionUser().UserName;
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["DeleteMark"] = "1";
                    ht["SysFlag"] = "0";
                }

                bool returnValue = articleManager.AddOrEditArticleInfo(ht, _key);
                if (returnValue)
                {
                    ShowMsgHelper.AlertMsg("操作成功！");
                }
                else
                {
                    ShowMsgHelper.Alert_Error("操作失败！");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}