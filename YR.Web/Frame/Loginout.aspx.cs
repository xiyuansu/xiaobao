using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetBean;

namespace YR.Web.Frame
{
    public partial class Loginout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //注销用户
            RequestSession.DisponsedSessionUser();
            Response.Redirect("~/Frame/Login.htm");
        }
    }
}