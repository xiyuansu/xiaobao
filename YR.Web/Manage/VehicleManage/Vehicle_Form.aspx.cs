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

namespace YR.Web.Manage.VehicleManage
{
    public partial class Vehicle_Form : PageBase
    {
        private VehicleManager vehicleManager = new VehicleManager();

        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        public DataTable VehicleImageTable
        {
            get
            {
                SystemImagesManager imagesManager = new SystemImagesManager();
                return imagesManager.GetImagesByVehicleID(this._key);
            }
        }

        public DataTable VehiclePlatformTable
        {
            get
            {
                PlatformManager platformManager = new PlatformManager();
                return platformManager.GetAllPlatformInfo();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {                 //主键
            //CreateUserName.Value = RequestSession.GetSessionUser().UserName.ToString();
            //CreateDate.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (!IsPostBack)
            {
                this.BindPlatforms();
                BindModel();
                BindCity();
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
            Hashtable ht = vehicleManager.GetVehicleInfoByID(_key);

            if (ht.Count > 0 && ht != null)
            {
                this.tab4.Visible = true;
                this.tab5.Visible = true;
                ControlBindHelper.SetWebControls(this.Page, ht);
                this.drpVModel.Value = ht["MODEL"].ToString();
            }
        }

        private void BindPlatforms()
        {
            PlatformManager platformManager = new PlatformManager();
            PlatformId.DataSource = platformManager.GetAllPlatformInfo();
            PlatformId.DataTextField = "Platform";
            PlatformId.DataValueField = "ID";
            PlatformId.DataBind();
        }

        private void BindModel()
        {
            VehicleModelManager vehicleModelManager = new VehicleModelManager();
            DataTable dtVM = vehicleModelManager.GetAllModelInfo();
            drpVModel.DataSource = dtVM;
            drpVModel.DataTextField = "Name";
            drpVModel.DataValueField = "ID";
            drpVModel.DataBind();
        }

        private void BindCity()
        {
            DictManager dictManager = new DictManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DataRow defaultRow = dt.NewRow();
            defaultRow["ID"] = "";
            defaultRow["Name"] = "";
            dt.Rows.Add(defaultRow);
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

                #region 初始化车辆信息
                // 车辆名称
                ht["Name"] = Name.Value;
                // 车辆名称
                ht["VehicleName"] = LicenseNumber.Value;
                // 车牌号
                ht["LicenseNumber"] = LicenseNumber.Value;
                // 车辆颜色
                ht["VehicleColor"] = VehicleColor.Value;
                // 车辆排量
                ht["Displacement"] = Displacement.Value;
                // 车架号
                ht["VehicleNum"] = VehicleNum.Value;
                // 车辆GPS编号
                ht["VehicleGPSNum"] = VehicleGPSNum.Value;
                // 燃料方式
                ht["FuelStyle"] = FuelStyle.Value;
                // 平台
                ht["PlatformId"] = PlatformId.Value;
                // 公里数
                ht["Mileage"] = Mileage.Value;
                // 车辆品牌
                ht["Brand"] = Brand.Value;
                // 车辆型号
                ht["Model"] = this.drpVModel.Value.ToString();
                // 发动机号
                ht["EngineNum"] = EngineNum.Value;
                // 使用状态
                ht["UseState"] = UseState.Value;
                // 车辆状态
                ht["VehicleState"] = VehicleState.Value;
                // 车辆描述
                ht["VehicleRemark"] = VehicleRemark.Value;
                // 价格体系
                ht["PriceSystem"] = PriceSystem.Value;
                // 用户须知
                ht["TransportInformation"] = TransportInformation.Value;
                // 预计满电续航里程
                ht["ExpectRange"] = ExpectRange.Value;
                // 预计最高时速
                ht["ExpectHighestSpeed"] = ExpectHighestSpeed.Value;
                // 电量
                ht["Electricity"] = "0";
                if (!string.IsNullOrEmpty(Electricity.Value))
                    ht["Electricity"] = Electricity.Value;
                // 续航里程
                ht["Range"] = "0";
                if (!string.IsNullOrEmpty(Range.Value))
                    ht["Range"] = Range.Value;
                //车辆手机号
                ht["VehicleMobile"] = VehicleMobile.Value;
                ht["CityID"] = CityID.Value;

                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                    ht["DeleteMark"] = "1";// 1 未删除，0 已删除
                    ht["Longitude"] = "0";
                    ht["Latitude"] = "0";
                }
                #endregion

                bool returnValue = vehicleManager.AddOrEditVehicleInfo(ht, _key);

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