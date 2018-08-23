using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using YR.Busines.IDAO;
using YR.Common.DotNetBean;
using YR.Busines;
using YR.Common.DotNetUI;
using YR.Common.DotNetData;
using YR.Common.DotNetCode;
using YR.Common.DotNetEncrypt;
using YR.Busines.DAL;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.UserManage
{
    public partial class User_NoReportLoss : PageBase
    {
        private string _key;
        private UserInfoManager userInfoManager = new UserInfoManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];                  //主键

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(_key))
                {
                    InitData();
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            DataTable dt = userInfoManager.GetReportLossInfoByID(_key);
            if (dt!=null &&dt.Rows.Count>0)
            {
                BindPhone.Value = dt.Rows[0]["BindPhone"].ToString();
                ReviewNote.Value = dt.Rows[0]["NoReportLossRemark"].ToString();
            }
        }

        #region 保存事件
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                Hashtable ht = new Hashtable();
                // ID
                ht["ID"] = this._key;
                // 绑定手机号
                ht["BindPhone"] = BindPhone.Value.Trim();
                // 解挂备注
                ht["NoReportLossRemark"]=ReviewNote.Value.Trim();
                // 解挂时间
                ht["NoReportLossTime"]=SiteHelper.GetWebServerCurrentTime();

                bool returnValue = userInfoManager.UserNoReportLoss(ht);

                if (returnValue)
                {
                    ShowMsgHelper.AlertMsg("操作成功！");
                }
                else
                {
                    ShowMsgHelper.Alert_Error("操作失败！");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}