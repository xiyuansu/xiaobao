using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using YR.Common.DotNetBean;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehiclePrice_Form : PageBase
    {
        private VehiclePriceManager manager = new VehiclePriceManager();

        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCity();
                BindModel();
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
            Hashtable ht = manager.GetPriceByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
                CityID.Disabled = true;
                ModelID.Disabled = true;
            }
        }

        private void BindCity()
        {
            DictManager dictManager = new DictManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataTable platformTable = dictManager.GetDictList("03");
            foreach (DataRow row in platformTable.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            CityID.DataSource = dt;
            CityID.DataTextField = "Name";
            CityID.DataValueField = "ID";
            CityID.DataBind();
        }

        private void BindModel()
        {
            VehicleModelManager modelManager = new VehicleModelManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataTable platformTable = modelManager.GetAllModelInfo();
            foreach (DataRow row in platformTable.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            ModelID.DataSource = dt;
            ModelID.DataTextField = "Name";
            ModelID.DataValueField = "ID";
            ModelID.DataBind();
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
                ht["ModelID"] = ModelID.Value;
                ht["CityID"] = CityID.Value;
                ht["MinutePrice"] = MinutePrice.Value;
                ht["KmPrice"] = KmPrice.Value;
                ht["MinPrice"] = MinPrice.Value;
                ht["MaxPrice"] = MaxPrice.Value;
                ht["Operator"] = RequestSession.GetSessionUser().UserName;
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                    ht["UpdateTime"] = DateTime.Now;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                    ht["DeleteMark"] = 1;
                }

                bool returnValue = manager.AddOrEdit(ht, _key);
                if (returnValue)
                {
                    ShowMsgHelper.AlertMsgNoFresh("操作成功！");
                }
                else
                {
                    ShowMsgHelper.Alert_Error("操作失败！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}