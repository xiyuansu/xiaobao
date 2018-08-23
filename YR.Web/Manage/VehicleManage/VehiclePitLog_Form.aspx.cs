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
    public partial class VehiclePitLog_Form : PageBase
    {
        private string _key;
        private VehicleMaintainLogManager vehicleMaintainLogManager = new VehicleMaintainLogManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];                  //主键

            if (!IsPostBack)
            {
                this.BindPits();
                this.BindVehicle();
                if (!string.IsNullOrEmpty(_key))
                {
                    InitData();
                }
            }
            // 判断是否为更换电池
            if (!string.IsNullOrEmpty(Request["vid"]))
            {
                this.VehicleID.Value = Request["vid"];
                this.VehicleID.Disabled = true;
                this.MaintainItems.Value = "更换电池";
                this.Remark.Value = "更换电池";
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            Hashtable ht = vehicleMaintainLogManager.GetVehicleMaintainLogInfoByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
            // 数据权限（当前用户为维修人员时默认显示为本维修部）
            if (IsDataAuthRole)
            {
                this.PitID.Value = GetDepartmentID();
            }
        }

        /// <summary>
        /// 绑定所属维修站
        /// </summary>
        private void BindPits()
        {
            PitsManager pitManager = new PitsManager();
            PitID.DataSource = pitManager.GetAllPitsInfo();
            PitID.DataTextField = "Name";
            PitID.DataValueField = "ID";
            PitID.DataBind();
        }

        /// <summary>
        /// 绑定车辆
        /// </summary>
        private void BindVehicle()
        {
            VehicleManager vehicleManager = new VehicleManager();
            VehicleID.DataSource = vehicleManager.GetAllVehicleInfo();
            VehicleID.DataTextField = "Name";
            VehicleID.DataValueField = "ID";
            VehicleID.DataBind();
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

                #region 初始化保养记录信息
                // 维修站ID
                ht["PitID"] = PitID.Value;
                // 保养车辆ID
                ht["VehicleID"] = VehicleID.Value;
                // 保养类型
                ht["MaintainType"] = "2";
                // 保养项目
                ht["MaintainItems"] = MaintainItems.Value;
                // 保养人
                ht["MaintainPeople"] = MaintainPeople.Value;
                // 保养人电话
                ht["LinkPhone"] = LinkPhone.Value;
                // 备注
                ht["Remark"] = Remark.Value;
                // 保养时间
                ht["MaintainTime"] = MaintainTime.Value;

                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                }
                #endregion

                bool returnValue = vehicleMaintainLogManager.AddOrEditVehicleMaintainLogInfo(ht, _key);

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