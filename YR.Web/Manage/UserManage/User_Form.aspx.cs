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
    public partial class User_Form : PageBase
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
                BindUserType();
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
                imgHeadPortrait.ImageUrl = ht["HEADPORTRAIT"] != null ? ht["HEADPORTRAIT"].ToString() : string.Empty;
                imgIDCard.ImageUrl = ht["IDCARD"] != null ? ht["IDCARD"].ToString() : string.Empty;
                //AppendProperty_value.Value = systemidao.GetPropertyInstancepk(Property_Function, _key);
            }
        }

        private void BindUserType()
        {
            UserInfoManager userManager = new UserInfoManager();
            DataTable dt= userManager.GetUserTypeList();
            UserType.DataSource = dt;
            UserType.DataTextField = "Name";
            UserType.DataValueField = "ID";
            UserType.DataBind();
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
            // 用户昵称
            ht["NickName"] = NickName.Value;
            // 真实名称
            ht["RealName"] = RealName.Value;
            // 绑定手机号
            ht["BindPhone"] = BindPhone.Value;
            // 头像地址
            //ht["HeadPortrait"] = imgphoto.ImageUrl;
            // 身份证号照片
            //ht["IDCard"] = txtIdCard.Value;
            // 驾驶证照片
            //ht["DriverLicense"] = txtDriverLicense.Value;
            // 实名认证状态
            ht["RealNameCertification"] = RealNameCertification.Value;
            // 账户余额
            //ht["Balance"] = Balance.Value;
            // 用户状态
            ht["UserState"] = UserState.Value;
            ht["UserType"] = UserType.Value;

            if (!string.IsNullOrEmpty(_key))
            {
                guid = _key;
            }
            else
            {
                ht["RegistrionTime"] = DateTime.Now;
            }
            bool returnValue = userManager.AddOrEditUserInfo(ht, _key);

            if (returnValue)
            {
                ShowMsgHelper.AlertMsgNoFresh("操作成功！");
            }
            else
            {
                ShowMsgHelper.Alert_Error("操作失败！");
            }
        }
        #endregion

    }
}