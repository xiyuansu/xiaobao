using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using YR.Common.DotNetUI;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysUserGroup
{
    public partial class UserGroupSet : PageBase
    {
        public string _UserGroup_ID, _UserGroup_Name;
        protected void Page_Load(object sender, EventArgs e)
        {
            _UserGroup_ID = Request["UserGroup_ID"];//主键
            _UserGroup_Name = Server.UrlDecode(Request["UserGroup_Name"]);//用户组名称
        }
    }
}