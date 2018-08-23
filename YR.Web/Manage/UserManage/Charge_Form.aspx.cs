using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetBean;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.UserManage
{
    public partial class Charge_Form : PageBase
    {
        string _key;
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
            RechargeRules userManager = new RechargeRules();
            Hashtable ht = userManager.GetByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
        }
        protected void Save_Click(object sender, EventArgs e)
        {
            RechargeRules userManager = new RechargeRules();
            Hashtable ht = new Hashtable();
            ht["Name"] = Name.Value;
            ht["ChargeMoney"] = ChargeMoney.Value;
            ht["PresentMoney"] = PresentMoney.Value;
            ht["Sort"] = Sort.Value;
            ht["BeginTime"] = BeginTime.Value;
            ht["EndTime"] = EndTime.Value;
            ht["Operator"] = RequestSession.GetSessionUser().UserName;
            if (!string.IsNullOrEmpty(_key))
            {
                ht["ID"] = _key;
                ht["UpdateTime"] = DateTime.Now;
            }
            else
            {
                ht["ID"] = CommonHelper.GetGuid;
                ht["CreateTime"] = DateTime.Now;
            }
            bool returnValue = userManager.AddOrEditSetting(ht,_key);
            if (returnValue)
            {
                ShowMsgHelper.AlertMsg("操作成功！");
            }
            else
            {
                ShowMsgHelper.Alert_Error("操作失败！");
            }
        }
    }
}