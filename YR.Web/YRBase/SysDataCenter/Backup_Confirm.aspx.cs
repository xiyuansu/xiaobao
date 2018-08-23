using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetBean;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysDataCenter
{
    public partial class Backup_Confirm : System.Web.UI.Page
    {
        public string _UserPwd;
        protected void Page_Load(object sender, EventArgs e)
        {
            _UserPwd = RequestSession.GetSessionUser().UserPwd.ToString();
        }
    }
}