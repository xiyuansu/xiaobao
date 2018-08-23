using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetBean;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;
using System.Data;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Common;


namespace YR.Web.Manage.VehicleManage
{
    public partial class VehicleModel_Form : System.Web.UI.Page
    {
        VehicleModelManager vModelManager = new VehicleModelManager();
        private string _key;
        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];
            if (!IsPostBack)
            {
                InitData();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            Hashtable ht = vModelManager.GetModelByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                this.hidId.Value = ht["ID"].ToString();
                this.txtName.Text = ht["NAME"].ToString();
                this.txtIntro.Text = ht["INTRO"].ToString();
                this.txtSort.Text = ht["SORT"].ToString();
                var htmlImag = "<img  src=\"" + ht["IMAGES"].ToString() + "\" />";
                this.uploadImg.InnerHtml = htmlImag;
                this.hidPath.Value = ht["IMAGES"].ToString();

                if (ht["DELETEMARK"].ToString().Equals("1"))
                {
                    this.radOK.Checked = true;
                    this.radNo.Checked = false;
                }
                else
                {
                    this.radOK.Checked = false;
                    this.radNo.Checked = true;
                }

            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            UserInfoManager uManage = new UserInfoManager();

            try
            {
                Hashtable ht = new Hashtable();

                // 条目名称
                ht["Name"] = this.txtName.Text.Trim();
                //图片地址
                ht["Images"] = this.hidPath.Value;
                //说明
                ht["Intro"] = this.txtIntro.Text.Trim();
                ht["Sort"] = this.txtSort.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                {
                    
                }
                else
                {
                  
                }

                if (string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                    ht["UpdateTime"] = DateTime.Now;
                }
                else
                {
                    ht["ID"] = _key;
                    ht["UpdateTime"] = DateTime.Now;
                   
                }
                // 状态（0未启用，1启用）
                if (radOK.Checked)
                {
                    ht["DeleteMark"] = "1";
                }
                else
                {
                    ht["DeleteMark"] = "0";
                }
                ht["Operator"] = RequestSession.GetSessionUser().UserName;
                bool result = vModelManager.AddOrEdit(ht, _key);
                if (result)
                {
                    ShowMsgHelper.AlertMsgNoFresh("操作成功！");
                }
                else
                {
                    ShowMsgHelper.Alert_Error("操作失败！");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetUploadImagePath()
        {
            return SiteHelper.GetAppsetString("UploadImagePath") + DateTime.Now.ToString("yyyyMMdd") + "/";
        }
    }
}