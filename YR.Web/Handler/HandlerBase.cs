using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using YR.Common.DotNetBean;

namespace YR.Web.Handler
{
    public class HandlerBase:IHttpHandler, IRequiresSessionState
    {
        public virtual void ProcessRequest(HttpContext context)
        {
            if (RequestSession.GetSessionUser() == null)
            {
                string action = context.Request["action"];   
                //判断一些请求不做非登录状态下跳转
                if (action != null)
                {
                    if (action.ToLower() != "login")
                    {
                        context.Session.Abandon();  //取消当前会话
                        context.Session.Clear();
                        context.Response.Redirect("~/Frame/Login.htm");
                    }
                }
                else
                {
                    context.Session.Abandon();  //取消当前会话
                    context.Session.Clear();
                    context.Response.Redirect("~/Frame/Login.htm");
                }
            }
        }

        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }
    }
}