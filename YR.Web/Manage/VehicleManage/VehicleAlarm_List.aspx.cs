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

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleAlarm_List : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
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
            VehicleAlarmManager alarmManager = new VehicleAlarmManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = alarmManager.GetList(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.PageIndex = pageIndex;
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
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
        /// <summary>
        /// 初始化查询条件
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<StringBuilder, IList<SqlParam>> InitCondition()
        {
            StringBuilder sb = new StringBuilder();
            List<SqlParam> IList_param = new List<SqlParam>();
            //车辆名称
            if (!string.IsNullOrEmpty(txtVehicleName.Value))
            {
                sb.Append(" and YR_Vehicles.LicenseNumber like @LicenseNumber ");
                IList_param.Add(new SqlParam("@LicenseNumber", '%' + txtVehicleName.Value.Trim() + '%'));
            }
            //异常类型
            if (selAlarmType.Value != "-1")
            {
                sb.Append(" and AlarmType=@AlarmType ");
                IList_param.Add(new SqlParam("@AlarmType", selAlarmType.Value));
            }
            //处理状态
            if (selAlarmStatus.Value != "-1")
            {
                sb.Append(" and AlarmStatus=@AlarmStatus ");
                IList_param.Add(new SqlParam("@AlarmStatus", selAlarmStatus.Value));
            }
            //起始创建时间
            if (!string.IsNullOrEmpty(txtStartCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCreateTime,YR_VehicleAlarms.CreateTime)>=0");
                IList_param.Add(new SqlParam("@StartCreateTime", txtStartCreateTime.Value));
            }
            //结束创建时间
            if (!string.IsNullOrEmpty(txtEndCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,YR_VehicleAlarms.CreateTime,@EndCreateTime)>=0 ");
                IList_param.Add(new SqlParam("@EndCreateTime", txtEndCreateTime.Value));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txtVehicleName.Value = "";
            selAlarmType.Value = "";
            selAlarmStatus.Value = "";
            txtEndCreateTime.Value = "";
            txtStartCreateTime.Value = "";
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

        public string GetAlarmType(string alarm_type)
        {
            //1无单移动,2断电,3倒地,4电子围栏越界,5离线
            string ret = "";
            switch(alarm_type)
            {
                case "1":
                    ret = "无单移动";
                    break;
                case "2":
                    ret = "断电";
                    break;
                //case "3":
                //    ret = "倒地";
                //    break;
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
            operatorname=alarmManager.GetOperator(operatorid, operator_type);
            if(operator_type=="1")
            {
                operatorname += "(后台操作员)";
            }
            else if(operator_type=="2")
            {
                operatorname += "(网点操作员)";
            }
            return operatorname;
        }

    }
}