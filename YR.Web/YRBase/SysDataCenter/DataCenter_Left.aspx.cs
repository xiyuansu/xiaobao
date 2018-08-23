using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using YR.Busines.DAL;
using YR.Busines.IDAO;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysDataCenter
{
    public partial class DataCenter_Left : PageBase
    {
        public StringBuilder treeItem_Table = new StringBuilder();
        YR_System_IDAO systemidao = new YR_System_Dal();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeTable();
            }
        }
        // <summary>
        /// 所以数据库列表
        /// </summary>
        public void GetTreeTable()
        {
            DataTable dt = systemidao.GetSysobjects();
            foreach (DataRow drv in dt.Rows)
            {
                treeItem_Table.Append("<li>");
                treeItem_Table.Append("<div onclick=\"GetTable('" + drv["TABLE_NAME"] + "')\"><img src=\"/Themes/Images/20130502112716785_easyicon_net_16.png\" width=\"16\" height=\"16\" />" + drv["TABLE_NAME"] + "</div>");
                treeItem_Table.Append("</li>");
            }
        }
    }
}