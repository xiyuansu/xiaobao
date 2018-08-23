using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using YR.Common.DotNetUI;
using YR.Busines;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using YR.Common.DotNetBean;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysPersonal
{
    public partial class HomeShortcut_List : PageBase
    {
        YR_System_IDAO sys_idao = new YR_System_Dal();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitBindData();
            }
        }
        /// <summary>
        /// 初始化绑定数据源
        /// </summary>
        private void InitBindData()
        {
            DataTable dt = sys_idao.GetHomeShortcut_List(RequestSession.GetSessionUser().UserId.ToString());
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
        }
    }
}