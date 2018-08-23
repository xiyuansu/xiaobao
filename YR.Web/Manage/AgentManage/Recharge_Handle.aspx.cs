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

namespace YR.Web.Manage.AgentManage
{
    public partial class Recharge_Handle : PageBase
    {
        private string _key;
        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];                  //主键

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
                UserRaiseWithdrawalManager userRaiseWithdrawalManager = new UserRaiseWithdrawalManager();
                Hashtable ht = new Hashtable();

                int successNum = 0;
                string[] strIDs = _key.Split(',');
                if (strIDs != null && strIDs.Length > 0)
                {
                    foreach (string id in strIDs)
                    {
                        successNum += userRaiseWithdrawalManager.UpdateUserRaiseWithdrawalState(id, State.Value, ReviewNote.Value);
                    }
                }
                ShowMsgHelper.AlertMsg("处理提现申请：" + strIDs.Length + "条，成功：" + successNum + "条");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}