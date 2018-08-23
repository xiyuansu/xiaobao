using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using YR.Common.DotNetUI;
using System.Data;
using YR.Busines;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysAppend
{
    public partial class AppendProperty_Function : PageBase
    {
        YR_System_IDAO systemidao = new YR_System_Dal();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            DataTable dt = systemidao.AppendProperty_Function();
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
        }
    }
}