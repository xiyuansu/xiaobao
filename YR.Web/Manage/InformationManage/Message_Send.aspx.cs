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

namespace YR.Web.Manage.InfomationManage
{
    public partial class Message_Form : PageBase
    {
        private string _key;
        private MessageManager messageManager = new MessageManager();
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
            //Hashtable ht = messageManager.GetNoReadMessagesByUser(_key);
            //if (ht.Count > 0 && ht != null)
            //{
            //    ControlBindHelper.SetWebControls(this.Page, ht);
            //}
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

                #region 初始化消息内容
                // 消息标题
                ht["MessageTitle"] = MessageTitle.Value;
                // 消息内容
                ht["MessageContent"] = MessageContent.Value;
                // 消息类型
                ht["MessageType"] = MessageType.System.GetHashCode();
                // 模块类型
                ht["ModuleType"] = ModuleType.Value;
                // 发布时间
                ht["ReleaseTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                // 操作人员ID
                ht["OperatorID"] = RequestSession.GetSessionUser().UserId.ToString();
                // 状态
                ht["State"] = MessageState.Enable.GetHashCode();
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                }
                #endregion

                bool returnValue = messageManager.AddOrEditMessageInfo(ht, _key);

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