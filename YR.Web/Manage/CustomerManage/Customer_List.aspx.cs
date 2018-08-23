using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.CustomerManage
{
    public partial class Customer_List : PageBase
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
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "SetFlag();", true);
        }
        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            CustomerManager hotelManager = new CustomerManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = hotelManager.GetCustomerPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
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
            //Category
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblCategory = e.Item.FindControl("lblCategory") as Label;
                if (lblCategory != null)
                {
                    string text = lblCategory.Text;
                    switch (text)
                    {
                        case "1":text="酒店";break;
	                    case "2":text="社区";break;
                        case "3":text="写字楼";break;
                        case "4":text="医院";break;
                        case "5":text="景区";break;
                        case "6":text="学校";break;
                        case "7": text = "企业单位"; break;
                        case "8": text = "集团客户"; break;
                    }
                    lblCategory.Text = text;
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

            // 名称
            if (!string.IsNullOrEmpty(txtName.Value))
            {
                sb.Append(" and Name like @name ");
                IList_param.Add(new SqlParam("@name", '%' + txtName.Value.Trim() + '%'));
            }
            // 地址
            if (!string.IsNullOrEmpty(txtAddress.Value))
            {
                sb.Append(" and Address like @Address ");
                IList_param.Add(new SqlParam("@Address", '%' + txtAddress.Value.Trim() + '%'));
            }
            //类型
            if (selCategory.Value != "-1")
            {
                sb.Append(" and Category=@Category ");
                IList_param.Add(new SqlParam("@Category", selCategory.Value));
            }
            // 固定电话
            if (!string.IsNullOrEmpty(txtPhone.Value))
            {
                sb.Append(" and Tel like @Phone ");
                IList_param.Add(new SqlParam("@Phone", '%' + txtPhone.Value.Trim() + '%'));
            }
            // 起始创建时间
            if (!string.IsNullOrEmpty(txtStartCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartCreateTime,CreateTime)>=0");
                IList_param.Add(new SqlParam("@StartCreateTime", txtStartCreateTime.Value));
            }
            // 结束创建时间
            if (!string.IsNullOrEmpty(txtEndCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,CreateTime,@EndCreateTime)>=0 ");
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
            txtAddress.Value = "";
            txtEndCreateTime.Value = "";
            txtStartCreateTime.Value = "";
            txtPhone.Value = "";
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            this.DataBindGrid(true);
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "SetFlag();", true);
        }
        // 重置
        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
            ScriptManager.RegisterStartupScript(UpdatePanel2, this.Page.GetType(), "", "SetFlag();", true);
        }
    }
}