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

namespace YR.Web.Manage.VehicleManage
{
    public partial class Platform_Form : PageBase
    {
        private PlatformManager platformeManager = new PlatformManager();

        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        public DataTable VehicleImageTable
        {
            get
            {
                SystemImagesManager imagesManager = new SystemImagesManager();
                return imagesManager.GetImagesByVehicleID(this._key);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {                 //主键
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
            Hashtable ht = platformeManager.GetPlatformInfoByID(_key);
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

                #region 初始化车辆信息
                
                // 平台名称
                ht["Platform"] = Platform.Value;
                // 平台协议代码
                ht["Code"] = Code.Value;
                // 平台说明
                ht["Memo"] = Memo.Value;
                ht["Sort"] = Sort.Value;

                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                }
                #endregion

                bool returnValue = platformeManager.AddOrEditPlatformInfo(ht, _key);

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