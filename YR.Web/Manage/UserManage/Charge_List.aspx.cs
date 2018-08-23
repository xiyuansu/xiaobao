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

namespace YR.Web.Manage.UserManage
{
    public partial class Charge_List : PageBase
    {
        public StringBuilder str_tableTree = new StringBuilder();

        RechargeRules user_idao = new RechargeRules();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                DataBindGrid();
            }
        }
        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            RechargeRules userManager = new RechargeRules();
            int count = 0;
            int pageIndex = 1;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();
            DataTable dt = userManager.GetInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
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
                /*Label lblUser_Sex = e.Item.FindControl("lblUser_Sex") as Label;
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
                }*/
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

            if (!string.IsNullOrEmpty(txtName.Value))
            {
                sb.Append(" and Name like @name");
                IList_param.Add(new SqlParam("@name", '%' + txtName.Value.Trim() + '%'));
            }
            if (!string.IsNullOrEmpty(txtBeginTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@BeginTime,BeginTime)>=0");
                IList_param.Add(new SqlParam("@BeginTime", txtBeginTime.Value));
            }
            if (!string.IsNullOrEmpty(txtEndTime.Value))
            {
                sb.Append(" and DATEDIFF(d,EndTime,@EndTime)>=0 ");
                IList_param.Add(new SqlParam("@EndTime", txtEndTime.Value));
            }
            if (!string.IsNullOrEmpty(txtCreateTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@CreateTime,CreateTime)>=0");
                IList_param.Add(new SqlParam("@CreateTime", txtCreateTime.Value));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void ClearInput()
        {
            txtName.Value = "";
            txtBeginTime.Value = "";
            txtEndTime.Value = "";
            txtCreateTime.Value = "";
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