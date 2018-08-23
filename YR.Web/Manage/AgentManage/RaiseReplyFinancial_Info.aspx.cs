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
    public partial class RaiseReplyFinancial_Info : PageBase
    {
        string _key;
        string Property_Function = "用户附加信息";
        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];                  //主键
            //CreateUserName.Value = RequestSession.GetSessionUser().UserName.ToString();
            //CreateDate.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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
            UserInfoManager userManager = new UserInfoManager();
            Hashtable ht = userManager.GetUserInfoByUserID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
                Password.Value = "*************";
                //AppendProperty_value.Value = systemidao.GetPropertyInstancepk(Property_Function, _key);
            }
        }

        /// <summary>
        /// 验证是否存在
        /// </summary>
        /// <param name="pkName">对象主键</param>
        /// <param name="Obj_id">对象主键值</param>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public string GetChecked(string pkName, string Obj_Val, DataTable dt)
        {
            StringBuilder strSql = new StringBuilder();
            dt = DataTableHelper.GetNewDataTable(dt, "" + pkName + " = '" + Obj_Val + "'");
            if (DataTableHelper.IsExistRows(dt))
                return "checked=\"checked\"";
            else
                return "";
        }

        #region 保存事件
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save_Click(object sender, EventArgs e)
        {
            UserInfoManager userManager = new UserInfoManager();
            string guid = CommonHelper.GetGuid;
            Hashtable ht = new Hashtable();
            // 用户名
            ht["UserName"] = UserName.Value;
            // 密码
            ht["Password"] = Md5Helper.MD5(Password.Value, 32);
            // 用户昵称
            ht["NickName"] = NickName.Value;
            // 头像地址
            ht["HeadPortrait"] = imgphoto.ImageUrl;
            // 真实名称
            ht["RealName"] = RealName.Value;
            // 绑定手机号
            ht["BindPhone"] = BindPhone.Value;
            // 身份证号照片
            ht["IDCard"] = txtIdCard.Value;
            // 驾驶证照片
            ht["DriverLicense"] = txtDriverLicense.Value;
            // 实名认证状态
            ht["RealNameCertification"] = RealNameCertification.Value;
            // 账户余额
            ht["Balance"] = Balance.Value;
            // 众筹模块余额
            ht["RaiseBalance"] = RaiseBalance.Value;
            // 用户状态
            ht["UserState"] = UserState.Value;

            if (!string.IsNullOrEmpty(_key))
            {
                guid = _key;
                ht.Remove("Password");
            }
            else
            {
                ht["RegistrionTime"] = DateTime.Now;
            }
            bool returnValue = userManager.AddOrEditUserInfo(ht, _key);

            if (returnValue)
            {
                ShowMsgHelper.AlertMsg("操作成功！");
            }
            else
            {
                ShowMsgHelper.Alert_Error("操作失败！");
            }
        }
        #endregion
    }
}