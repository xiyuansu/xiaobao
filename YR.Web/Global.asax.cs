using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using YR.Common.DotNetCode;
using System.Diagnostics;
using System.IO;
using System.Text;
using YR.Common.DotNetConfig;
using YR.Web.api.api_class;
using Newtonsoft.Json.Linq;
using System.Xml;
using YR.Common.DotNetLog;

namespace YR.Web
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// 创建系统异常日志
        /// </summary>
        private static Log Logger = LogFactory.GetLogger(typeof(Global));

        private Quartz.IScheduler sched = null;

        void Application_Start(object sender, EventArgs e)
        {
            //---------------------------初始化quartz---------------------------
            Quartz.ISchedulerFactory sf = new Quartz.Impl.StdSchedulerFactory();
            sched = sf.GetScheduler();
            sched.Start();
            //------------------------------------------------------------------

            Logger.Info("web启动成功");
        }

        void Application_End(object sender, EventArgs e)
        {
            //-----------------------关闭quartz----------------------------------
            sched.Shutdown();
            //-------------------------------------------------------------------

            Logger.Info("web关闭成功");
        }

        /// <summary>
        /// 错误处理页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Error(object sender, EventArgs e)
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            string error = objErr.Message + "";
            Server.ClearError();
            Application["error"] = error;
            Response.Redirect("~/Error/ErrorPage.aspx");
        }

        void Session_Start(object sender, EventArgs e)
        {

        }

        void Session_End(object sender, EventArgs e)
        {

        }
    }
}