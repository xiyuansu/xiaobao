using System;
using System.Collections;
using System.Data;
using YR.Common.DotNetUI;
using YR.Common.DotNetCode;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.VehicleManage
{
    public partial class ServiceArea_Form : PageBase
    {
        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        private ServiceAreaManager serviceAreaManager = new ServiceAreaManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCity();
                if (!string.IsNullOrEmpty(_key))
                {
                    InitData();
                }
            }
        }

        private void BindCity()
        {
            DictManager dict = new DictManager();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataRow newRow = dt.NewRow();
            DataTable city_dt = dict.GetDictList("03");
            foreach (DataRow row in city_dt.Rows)
            {
                newRow = dt.NewRow();
                newRow["ID"] = row["ID"].ToString();
                newRow["Name"] = row["Name"];
                dt.Rows.Add(newRow);
            }
            CityID.DataSource = dt;
            CityID.DataTextField = "Name";
            CityID.DataValueField = "ID";
            CityID.DataBind();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            Hashtable ht = serviceAreaManager.GetInfoByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
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
                ht["ThisName"] = ThisName.Value;
                ht["Longitude"] = Longitude.Value;
                ht["Latitude"] = Latitude.Value;
                ht["CityID"] = CityID.Value;
                ht["AreaType"] = AreaType.Value;
                ht["Address"] = Address.Value;
                ht["Status"] = Status.Value;
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["Status"] ="0";
                    ht["CreateTime"] = DateTime.Now;
                    ht["DeleteMark"] = "1";
                }
                bool returnValue = serviceAreaManager.AddOrEditInfo(ht, _key);
                if (returnValue)
                {
                    ShowMsgHelper.AlertMsgNoFresh("操作成功！");
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