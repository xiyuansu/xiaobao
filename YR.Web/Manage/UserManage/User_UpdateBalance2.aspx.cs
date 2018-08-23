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
using Asiasofti.SmartVehicle.Common.Enum;

namespace YR.Web.Manage.UserManage
{
    public partial class User_UpdateBalance2 : PageBase
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
                NickName.InnerText = ht["NICKNAME"].ToString();
                RealName.InnerText = ht["REALNAME"].ToString();
                Balance.InnerText = ht["BALANCE2"].ToString();
                EndBalance.InnerText = ht["BALANCE2"].ToString();
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
                ht["ID"] = CommonHelper.GetGuid;
                ht["UserID"] = _key;
                // 订单号
                ht["OrderNum"]="";    
                // 变动类型
                ht["ChangesType"] = OprType.Value;
                // 变动金额
                ht["ChangesAmount"] = OprType.Value == UserFinancialChangesType.PlatMinus.GetHashCode().ToString() ? "-" + AdjustmentMoney.Value : AdjustmentMoney.Value;
                ht["OprMoney"] = AdjustmentMoney.Value;
                // 变动时间
                ht["ChangesTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                // 备注
                ht["Remark"] = txtRemark.Value.Trim();
                // 操作者ID
                ht["Operator"] = RequestSession.GetSessionUser().UserId.ToString(); ;
                // 操作者类型
                ht["OperatorType"] = UserFinancialOperatorType.Admin.GetHashCode();
                // 操作方式
                ht["OperatorWay"] = UserFinancialOperatorWay.Plat.GetHashCode();
                // 状态
                ht["State"] = UserFinancialState.Effect.GetHashCode();
                bool returnValue = false;
                // 更新用车用户余额
                if (!string.IsNullOrEmpty(AdjustmentMoney.Value.Trim()))
                {
                    returnValue = userInfoManager.UpdateUserBalance2(ht);
                }

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