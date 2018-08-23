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
using Asiasofti.SmartVehicle.Common.Enum;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleFault_Form : PageBase
    {
        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        private VehicleFaultManager faultManager = new VehicleFaultManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(_key))
                {
                    this.InitData();
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            DataTable dt = faultManager.GetVehicleFaultInfoByID(_key);
            if (dt.Rows.Count > 0 && dt != null)
            {
                this.VehicleName.Value = dt.Rows[0]["VehicleName"].ToString();
                this.RealName.Value = dt.Rows[0]["SubmitUserName"].ToString();
                this.FaultType.Value = dt.Rows[0]["FaultType"].ToString();
                this.selTriggerType.Value = dt.Rows[0]["TriggerType"].ToString();
                this.Remark.Value = dt.Rows[0]["Remark"].ToString();
                this.VehicleID.Value = dt.Rows[0]["VehicleName"].ToString();
                this.UserID.Value = dt.Rows[0]["SubmitUserID"].ToString();

                string images= dt.Rows[0]["FaultImages"].ToString();
                string img_html = "";
                foreach (string img in images.Split(new string[] { ";"}, StringSplitOptions.RemoveEmptyEntries))
                {
                    img_html += "<img src='"+img+"' width='100' height='100'/>";
                }
                this.img_list.Text = img_html;

                if(dt.Rows[0]["FaultDoState"].ToString()==VehicleFaultDoState.Disposed.GetHashCode().ToString())
                {
                    Save.Visible = false;
                    Save.Enabled = false;
                }
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
                ht["FaultDoState"] = (int)VehicleFaultDoState.Disposed;
                ht["FaultDoTime"] = SiteHelper.GetWebServerCurrentTime();
                ht["FaultDoUserID"] = RequestSession.GetSessionUser().UserId;
                ht["FaultDoUserName"] = RequestSession.GetSessionUser().UserName;
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                }

                bool returnValue = faultManager.AddOrEditVehicleFaultInfo(ht, _key);
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