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
    public partial class VehicleCommentsLog_Audit : PageBase
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
                Hashtable ht = new Hashtable();

                // 状态
                ht["State"] = selState.Value;
                // 审核描述
                ht["ReviewNote"] = txtRemark.Value;

                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                VehicleCommentsLogManager vehicleCommentsManager = new VehicleCommentsLogManager();

                bool returnValue = vehicleCommentsManager.AddOrEditVehicleCommentsLogInfo(ht, _key);

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