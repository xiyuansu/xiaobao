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

namespace YR.Web.Manage.InfomationManage
{
    public partial class Sms_Form : PageBase
    {
        private string _key;
        private PitsManager pitsManager = new PitsManager();
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
            Hashtable ht = pitsManager.GetPitsInfoByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
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
            try
            {
                Hashtable ht = new Hashtable();

                #region 初始化维修站信息
                // 名称
                ht["Name"] = Name.Value;
                // 地址
                ht["Address"] = Address.Value;
                // 固定电话
                ht["TelePhone"] = Telephone.Value;
                // 手机号
                ht["Mobile"] = Mobile.Value;
                // 经度
                ht["Longitude"] = Longitude.Value;
                // 纬度
                ht["Latitude"] = Latitude.Value;
                // 停车场描述
                ht["Description"] = Description.Value;
                // 操作人员ID
                ht["OwnerID"] = RequestSession.GetSessionUser().UserId.ToString();
                // 状态（1可用 2不可用）
                ht["State"] = "1";

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

                bool returnValue = false;

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