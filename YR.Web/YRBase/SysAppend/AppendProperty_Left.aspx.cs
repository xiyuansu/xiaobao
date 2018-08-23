using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using YR.Busines;
using YR.Common.DotNetData;
using YR.Busines.DAL;
using YR.Busines.IDAO;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysAppend
{
    public partial class AppendProperty_Left : PageBase
    {
        public StringBuilder strHtml = new StringBuilder();
        YR_System_IDAO systemidao = new YR_System_Dal();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitInfo();
            }
        }
        /// <summary>
        /// 所属功能
        /// </summary>
        public void InitInfo()
        {
            DataTable dt = systemidao.AppendProperty_Function();
            if (DataTableHelper.IsExistRows(dt))
            {
                DataView dv = new DataView(dt);
                foreach (DataRowView drv in dv)
                {
                    strHtml.Append("<li>");
                    strHtml.Append("<div onclick=\"Property_Function('" + drv["Property_Function"].ToString() + "')\">" + drv["Property_Function"].ToString() + "</div>");
                    strHtml.Append("</li>");
                }
            }
            else
            {
                strHtml.Append("<li>");
                strHtml.Append("<div><span style='color:red;'>暂无数据</span></div>");
                strHtml.Append("</li>");
            }
        }
    }
}