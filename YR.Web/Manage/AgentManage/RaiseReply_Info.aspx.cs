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
    public partial class RaiseReply_Info : PageBase
    {
        private string _key;
        private UserRaiseReplyManager raiseReplyManager = new UserRaiseReplyManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];                  //主键

            if (!IsPostBack)
            {
                this.BindUser();
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
            Hashtable ht = raiseReplyManager.GetUserRaiseReplyByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
        }

        /// <summary>
        /// 绑定所属会员
        /// </summary>
        private void BindUser()
        {
            UserInfoManager userInfoManager = new UserInfoManager();
            UserID.DataSource = userInfoManager.GetUserListByCondition();
            UserID.DataTextField = "NickName";
            UserID.DataValueField = "ID";
            UserID.DataBind();
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
            try
            {
                Hashtable ht = new Hashtable();

                #region 初始化众筹申请信息
                // 会员
                ht["UserID"] = UserID.Value;
                // 名称
                ht["Name"] = Name.Value;
                // 联系电话
                ht["LinkPhone"] = LinkPhone.Value;
                // 车辆图片1
                ht["VehiclePic1"] = VehiclePic1.Value;
                // 车辆图片2
                ht["VehiclePic2"] = VehiclePic1.Value;
                // 车辆图片3
                ht["VehiclePic3"] = VehiclePic1.Value;
                // 车辆描述
                ht["VehicleRemark"] = VehicleRemark.Value;

                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                }
                #endregion

                bool returnValue = raiseReplyManager.AddOrEditUserRaiseReplyInfo(ht, _key);

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