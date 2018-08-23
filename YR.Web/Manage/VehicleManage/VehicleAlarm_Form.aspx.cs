using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetBean;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleAlarm_Form : PageBase
    {
        private VehicleAlarmManager alarmManager = new VehicleAlarmManager();

        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        private string _action
        {
            get
            {
                return Request["action"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(_key))
                {
                    if(_action=="view")
                    {
                        Save.Visible = false;
                    }
                    else if(_action=="edit")
                    {
                        Save.Visible = true;
                    }
                    InitData();
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            Hashtable ht = alarmManager.GetInfoByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                VehicleName.InnerText = ht["VEHICLENAME"].ToString();
                AlarmType.InnerText = GetAlarmType(ht["ALARMTYPE"].ToString());
                AlarmTime.InnerText = ht["ALARMTIME"].ToString();
                AlarmStatus.InnerText = GetAlarmStatus(ht["ALARMSTATUS"].ToString());
                OperateTime.InnerText = ht["OPERATETIME"].ToString();
                OperatorID.InnerText = GetOperator(ht["OPERATORID"].ToString(), ht["OPERATORTYPE"].ToString());
                OperateRemark.InnerText = ht["OPERATEREMARK"].ToString();
                if(ht["ALARMSTATUS"].ToString().CompareTo("1")==0)
                {
                    OperateRemark.Disabled = true;
                    Save.Visible = false;
                }
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
                ht["ID"] = _key;
                ht["OperateRemark"] = OperateRemark.Value;
                ht["OperateTime"] = SiteHelper.GetWebServerCurrentTime();
                ht["OperatorID"] = RequestSession.GetSessionUser().UserId.ToString();
                ht["OperatorType"] = 1;
                ht["AlarmStatus"] = 1;
                bool returnValue = alarmManager.AddOrEdit(ht, _key);
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

        public string GetAlarmType(string alarm_type)
        {
            //1无单移动,2断电,3倒地,4电子围栏越界,5离线
            string ret = "";
            switch (alarm_type)
            {
                case "1":
                    ret = "无单移动";
                    break;
                case "2":
                    ret = "断电";
                    break;
                case "3":
                    ret = "倒地";
                    break;
                case "4":
                    ret = "越界";
                    break;
                case "5":
                    ret = "离线";
                    break;
                case "6":
                    ret = "超速";
                    break;
                case "7":
                    ret = "振动";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public string GetAlarmStatus(string alarm_status)
        {
            //异常状态,0未处理,1已处理
            string ret = "";
            switch (alarm_status)
            {
                case "0":
                    ret = "未处理";
                    break;
                case "1":
                    ret = "已处理";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public string GetOperator(string operatorid, string operator_type)
        {
            //operator_type:异常处理人类型，1后台操作员，2网点操作员
            string operatorname = "";
            VehicleAlarmManager alarmManager = new VehicleAlarmManager();
            operatorname = alarmManager.GetOperator(operatorid, operator_type);
            if (operator_type == "1")
            {
                operatorname += "(后台操作员)";
            }
            else if (operator_type == "2")
            {
                operatorname += "(网点操作员)";
            }
            return operatorname;
        }

    }
}