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
using Asiasofti.SmartVehicle.Common.Enum;

namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleMaintainLog_List : PageBase
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
        }
        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            VehicleMaintainLogManager vehicleMaintainLogManager = new VehicleMaintainLogManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();

            DataTable dt = vehicleMaintainLogManager.GetVehicleMaintainLogInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
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
            sb.Append(" and a.MaintainType=" + VehicleMaintainLog.Maintenance.GetHashCode());

            // 维修站名称
            if (!string.IsNullOrEmpty(txtName.Value))
            {
                sb.Append(" and b.Name like @name ");
                IList_param.Add(new SqlParam("@name", '%' + txtName.Value.Trim() + '%'));
            }
            // 维修车辆
            if (!string.IsNullOrEmpty(txtVehicleName.Value))
            {
                sb.Append(" and c.VehicleName like @VehicleName ");
                IList_param.Add(new SqlParam("@VehicleName", '%' + txtVehicleName.Value.Trim() + '%'));
            }
            // 维修项目
            if (!string.IsNullOrEmpty(txtMaintainItems.Value))
            {
                sb.Append(" and a.MaintainItems like @MaintainItems ");
                IList_param.Add(new SqlParam("@MaintainItems", '%' + txtMaintainItems.Value.Trim() + '%'));
            }
            // 维修人
            if (!string.IsNullOrEmpty(txtMaintainPeople.Value))
            {
                sb.Append(" and a.MaintainPeople like @MaintainPeople ");
                IList_param.Add(new SqlParam("@MaintainPeople", '%' + txtMaintainPeople.Value.Trim() + '%'));
            }
            // 维修人电话
            if (!string.IsNullOrEmpty(txtLinkPhone.Value))
            {
                sb.Append(" and a.LinkPhone like @LinkPhone ");
                IList_param.Add(new SqlParam("@LinkPhone", '%' + txtLinkPhone.Value.Trim() + '%'));
            }
            //// 状态
            //if (selState.Value != "-1")
            //{
            //    sb.Append(" and a.State=@State ");
            //    IList_param.Add(new SqlParam("@State", selState.Value));
            //}
            // 起始创建时间
            if (!string.IsNullOrEmpty(txtStartCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCreateTime,a.MaintainTime)>=0");
                IList_param.Add(new SqlParam("@StartCreateTime", txtStartCreateTime.Value));
            }
            // 结束创建时间
            if (!string.IsNullOrEmpty(txtEndCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,a.MaintainTime,@EndCreateTime)>=0 ");
                IList_param.Add(new SqlParam("@EndCreateTime", txtEndCreateTime.Value));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txtName.Value = "";
            txtVehicleName.Value = "";
            txtEndCreateTime.Value = "";
            txtStartCreateTime.Value = "";
            txtMaintainItems.Value = "";
            txtMaintainPeople.Value = "";
            txtLinkPhone.Value = "";
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            this.DataBindGrid(true);
        }
        // 重置
        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
        }
    }
}