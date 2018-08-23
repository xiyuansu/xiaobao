using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetBean;
using System.Web.UI;
using System.Collections;
using YR.Common.DotNetUI;
using System.Data;
using System.Text;
using YR.Common.DotNetCode;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using Asiasofti.SmartVehicle.Common;

namespace YR.Web.App_Code
{
    /// <summary>
    /// 基类
    /// </summary>
    public class PageBase : System.Web.UI.Page
    {
        YR_System_IDAO sys_idao = new YR_System_Dal();
        /// <summary>
        /// 获取使用数据权限的角色名称，判断当前角色是否满足
        /// </summary>
        public bool IsDataAuthRole
        {
            get
            {
                string roleName = RequestSession.GetSessionUser().RoleName.ToString();
                string dataRoleNames = SiteHelper.GetAppsetString("DataAuthDept");
                return dataRoleNames.IndexOf(roleName) > -1 ? true : false;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            #region 当Session过期自动跳出登录画面
            if (RequestSession.GetSessionUser() == null)
            {
                Session.Abandon();  //取消当前会话
                Session.Clear();
                Response.Redirect("/Index.htm");
            }
            #endregion

            #region 防止刷新重复提交
            if (null == Session["Token"])
            {
                WebHelper.SetToken();
            }
            #endregion

            #region URL权限验证,拒绝，不合法的请求
            URLPermission();
            #endregion

            base.OnLoad(e);
        }
        #region URL权限验证,拒绝，不合法的请求
        /// <summary>
        /// URL权限验证,拒绝，不合法的请求
        /// </summary>
        public void URLPermission()
        {
            bool IsOK = false;
            //获取当前访问页面地址
            string requestPath = RequestHelper.GetScriptName;
            string[] filterUrl = { "/Frame/HomeIndex.aspx", "/YRBase/SysUser/UpdateUserPwd.aspx" };//过滤特别页面
            //对上传的文件的类型进行一个个匹对
            for (int i = 0; i < filterUrl.Length; i++)
            {
                if (requestPath == filterUrl[i])
                {
                    IsOK = true;
                    break;
                }
            }
            if (!IsOK)
            {
                string UserId = RequestSession.GetSessionUser().UserId.ToString();//用户ID
                DataTable dt = sys_idao.GetPermission_URL(UserId);
                DataView dv = new DataView(dt);
                dv.RowFilter = "NavigateUrl = '" + requestPath + "'";
                if (dv.Count == 0)
                {
                    StringBuilder strHTML = new StringBuilder();
                    strHTML.Append("<div style='text-align: center; line-height: 300px;'>");
                    strHTML.Append("<font style=\"font-size: 13;font-weight: bold; color: red;\">权限不足</font></div>");
                    HttpContext.Current.Response.Write(strHTML.ToString());
                    HttpContext.Current.Response.End();
                }
            }

        }
        #endregion

        #region 判断当前登录用户角色（如业务角色（维修部）则过滤数据范围）
        public string GetDepartmentID()
        {
            string userID=  RequestSession.GetSessionUser().UserId.ToString();
            YR_UserInfo_Dal userInfoDal = new YR_UserInfo_Dal();
            return userInfoDal.GetPitsIDByUserID(userID);
        }
        #endregion
    }
}