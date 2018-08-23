using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;

namespace YR.Common.DotNetUI
{
    /// <summary>
    /// 客户端提示信息帮助类
    /// </summary>
    public class ShowMsgHelper
    {
        /// <summary>
        /// 默认成功提示
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void Alert(string message)
        {
            ExecuteScript(string.Format("showTipsMsg('{0}','2500','4');", message));
        }
        /// <summary>
        /// 默认成功提示，刷新父窗口函数关闭页面
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void AlertMsg(string message)
        {
            ExecuteScript(string.Format("showTipsMsg('{0}','2500','4');top.main.windowload();OpenClose();", message));
        }
        /// <summary>
        /// 成功提示，调用父窗口load函数执行查询，不刷新父窗口，关闭页面
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void AlertMsgNoFresh(string message)
        {
            ExecuteScript(string.Format("showTipsMsg('{0}','2500','4');top.main.load();OpenClose();", message));
        }
        /// <summary>
        /// 默认成功提示，刷新父窗口函数关闭页面
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void ParmAlertMsgNoFresh(string message)
        {
            ExecuteScript(string.Format("showTipsMsg('{0}','2500','4');top.main.target_right.load();OpenClose();", message));
        }
        public static void ParmAlertMsg(string message)
        {
            ExecuteScript(string.Format("showTipsMsg('{0}','2500','4');top.main.target_right.windowload();OpenClose();", message));
        }
        /// <summary>
        /// 默认错误提示
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void Alert_Error(string message)
        {
            ExecuteScript(string.Format("showTipsMsg('{0}','5000','10');", message));
        }
        /// <summary>
        /// 默认警告提示
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void Alert_Wern(string message)
        {
            ExecuteScript(string.Format("showTipsMsg('{0}','3000','3');", message));
        }
        /// <summary>
        /// 提示警告信息
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void showFaceMsg(string message)
        {
            ExecuteScript(string.Format("showFaceMsg('{0}');", message));
        }
        /// <summary>
        /// 提示警告信息
        /// </summary>
        /// <param name="message">显示消息</param>
        public static void showWarningMsg(string message)
        {
            ExecuteScript(string.Format("showWarningMsg('{0}');", message));
        }

        /// <summary>
        /// 后台调用JS函数
        /// </summary>
        /// <param name="obj"></param>
        public static void ShowScript(string strobj)
        {
            Page p = HttpContext.Current.Handler as Page;
            p.ClientScript.RegisterStartupScript(p.ClientScript.GetType(), "myscript", "<script>" + strobj + "</script>");
        }
        public static void ExecuteScript(string scriptBody)
        {
            string scriptKey = "Somekey";
            Page p = HttpContext.Current.Handler as Page;
            p.ClientScript.RegisterStartupScript(typeof(string), scriptKey, scriptBody, true);
        }
    }
}
