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
    public partial class User_Audit : PageBase
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
            Hashtable ht = userInfoManager.GetUserInfoByUserID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
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

                // 实名认证状态
                ht["RealNameCertification"] = RealNameCertification.Value;
                // 审核描述
                ht["ReviewNote"] = ReviewNote.Value;

                //if (!string.IsNullOrEmpty(_key))
                //{
                //    ht["ID"] = _key;
                //}

                string userIDStr = Request["key"];
                string[] userID = new string[] { };
                if (!string.IsNullOrEmpty(userIDStr))
                {
                    userID = userIDStr.Split(',');
                }
                int sumNum = userID.Length; // 总数量
                int succNum = 0;            // 成功数量
                int failNum = 0;            // 失败数量
                foreach (string uID in userID)
                {
                    ht["ID"] = uID;
                    if (userInfoManager.AddOrEditUserInfo(ht, uID))
                    {
                        succNum += 1;
                    }
                    else
                    {
                        failNum += 1;
                    }
                }

                ShowMsgHelper.AlertMsg("审核用户数量" + sumNum + "，成功" + succNum + "，失败" + failNum);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}