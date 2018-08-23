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
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.CustomerManage
{
    public partial class ServiceStation_Form : PageBase
    {
        private ServiceStationManager manager = new ServiceStationManager();

        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        public DataTable CustomerTable
        {
            get
            {
                CustomerManager customerManager = new CustomerManager();
                return customerManager.GetAllCustomers();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.BindHotel();
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
            Hashtable ht = manager.GetServiceStationByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
        }

        private void BindHotel()
        {
            if (Request["ID"] != null && Request["ID"].ToString().Trim().Length > 0 && Request["Name"] != null && Request["Name"].ToString().Trim().Length > 0 && Request["address"] != null && Request["address"].ToString().Trim().Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", Type.GetType("System.String"));
                dt.Columns.Add("Name", Type.GetType("System.String"));
                dt.Columns.Add("Address", Type.GetType("System.String"));
                DataRow newRow = dt.NewRow();
                newRow["ID"] = Request["ID"].ToString();
                newRow["Name"] = Request["Name"].ToString();
                newRow["Address"] = Request["address"].ToString();
                dt.Rows.Add(newRow);
                CustomerID.DataSource = dt;
                CustomerID.DataTextField = "Name";
                CustomerID.DataValueField = "ID";
                CustomerID.DataBind();
                Name.Value = Request["Name"].ToString();
                Address.Value = Request["address"].ToString();
            }
            else
            {
                CustomerID.DataSource = CustomerTable;
                CustomerID.DataTextField = "Name";
                CustomerID.DataValueField = "ID";
                CustomerID.DataBind();
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
                ht["Name"] = Name.Value;
                ht["Address"] = Address.Value;
                ht["Contacts"] = Contacts.Value;
                ht["ContactTel"] = ContactTel.Value;
                ht["Manager"] = Managers.Value;
                ht["ManagerTel"] = ManagerTel.Value;
                ht["Longitude"] = "0";
                if(!string.IsNullOrEmpty(Longitude.Value))
                    ht["Longitude"] = Longitude.Value;
                ht["Latitude"] = "0";
                if (!string.IsNullOrEmpty(Latitude.Value))
                    ht["Latitude"] = Latitude.Value;
                //ht["ParkingRadius"] = string.IsNullOrEmpty(ParkingRadius.Value.Trim()) ? "0.00" : ParkingRadius.Value.Trim();
                ht["CustomerID"] = CustomerID.Value;
                ht["State"] = State.Value;
                ht["Createtime"] = DateTime.Now;
                if(!string.IsNullOrEmpty(Longitude.Value) && !string.IsNullOrEmpty(Latitude.Value))
                {
                    Hashtable loc_ht= manager.GetProvinceCityDistrictCode(Longitude.Value, Latitude.Value);
                    if(loc_ht!=null && loc_ht.Keys.Count>0)
                    {
                        ht["ProvinceID"] = SiteHelper.GetHashTableValueByKey(loc_ht, "ProvinceID");
                        ht["CityID"] = SiteHelper.GetHashTableValueByKey(loc_ht, "CityID");
                        ht["CountyID"] = SiteHelper.GetHashTableValueByKey(loc_ht, "CountyID");
                    }
                }

                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                }

                bool returnValue = manager.AddOrEditServiceStation(ht, _key);

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