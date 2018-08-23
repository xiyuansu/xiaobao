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

namespace YR.Web.Manage.UserManage
{
    public partial class VehicleHistory_List : PageBase
    {
        VehicleHistoryManager vehicleHistoryManager = new VehicleHistoryManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                BindControl();
                DataBindGrid();
            }
        }

        /// <summary>
        /// 绑定控件
        /// </summary>
        private void BindControl()
        {
            //UserInfoManager userInfoManager = new UserInfoManager();
            //DataTable dt = userInfoManager.GetAllUser();
            //if (dt != null)
            //{
            //    selUser.DataSource = dt;
            //    selUser.DataTextField = "UserName";
            //    selUser.DataValueField = "ID";
            //    selUser.DataBind();
            //}
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
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();

            DataTable dt = vehicleHistoryManager.GetVehicleHistoryInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
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
                Label lblUser_Sex = e.Item.FindControl("lblUser_Sex") as Label;
                Label lblDeleteMark = e.Item.FindControl("lblDeleteMark") as Label;
                if (lblUser_Sex != null)
                {
                    string text = lblUser_Sex.Text;
                    text = text.Replace("1", "男士");
                    text = text.Replace("0", "女士");
                    lblUser_Sex.Text = text;

                    string textDeleteMark = lblDeleteMark.Text;
                    textDeleteMark = textDeleteMark.Replace("1", "<span style='color:Blue'>启用</span>");
                    textDeleteMark = textDeleteMark.Replace("2", "<span style='color:red'>停用</span>");
                    lblDeleteMark.Text = textDeleteMark;
                }
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

            // 车辆名称
            if (!string.IsNullOrEmpty(txtVehicleName.Value))
            {
                sb.Append(" and c.LicenseNumber like @LicenseNumber ");
                IList_param.Add(new SqlParam("@LicenseNumber", '%' + txtVehicleName.Value.Trim() + '%'));
            }
            // 操作类型
            if (selDriveStyle.Value != "-1")
            {
                sb.Append(" and a.DriveStyle=@DriveStyle ");
                IList_param.Add(new SqlParam("@DriveStyle", selDriveStyle.Value));
            }
            // 操作结果
            if (selOprResult.Value != "-1")
            {
                sb.Append(" and a.OprResult=@OprResult ");
                IList_param.Add(new SqlParam("@OprResult", selOprResult.Value));
            }
            // 行驶起始时间
            if (!string.IsNullOrEmpty(txtStartDrivingTime.Value))
            {
                sb.Append(" and a.RecordTime>=@StartDrivingTime ");
                IList_param.Add(new SqlParam("@StartDrivingTime", txtStartDrivingTime.Value+" 00:00:00"));
            }
            // 行驶结束时间
            if (!string.IsNullOrEmpty(txtEndDrivingTime.Value))
            {
                sb.Append(" and a.RecordTime<=@EndDrivingTime ");
                IList_param.Add(new SqlParam("@EndDrivingTime", txtEndDrivingTime.Value +" 23:59:59"));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void ClearInput()
        {
            txtVehicleName.Value = "";
            txtStartDrivingTime.Value = "";
            txtEndDrivingTime.Value = "";
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

        public string GetDriveStyle(string operate)
        {
            //01开车，02锁车，03寻车，04开座锁
            string ret = "";
            switch (operate)
            {
                case "01":
                    ret = "开车";
                    break;
                case "02":
                    ret = "锁车";
                    break;
                case "03":
                    ret = "寻车";
                    break;
                case "04":
                    ret = "开座锁";
                    break;
            }
            return ret;
        }

        public string GetPower(string power)
        {
            //01开车，02锁车，03寻车，04开座锁
            string ret = "";
            if (string.IsNullOrEmpty(power))
            {
                ret = "";
            }
            else
            {
                decimal value = 0.00m;
                decimal.TryParse(power, out value);
                ret = string.Format("{0}%",(int)value);
            }
            return ret;
        }
    }
}