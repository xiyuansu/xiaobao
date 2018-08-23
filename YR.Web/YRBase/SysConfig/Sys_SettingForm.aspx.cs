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
using YR.Common.DotNetLog;

namespace YR.Web.YRBase.SysConfig
{
    public partial class Sys_SettingForm : PageBase
    {
        private static Log Logger = LogFactory.GetLogger(typeof(Sys_SettingForm));
        SysSettingManager sysSetingManager = new SysSettingManager();
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
            DataTable dtDic = sysSetingManager.GetBaseDictList();
            if (dtDic.Rows.Count > 0)
            {
                ////网点马厩绑定
                drpDic.DataSource = dtDic;
                drpDic.DataTextField = "Name";
                drpDic.DataValueField = "ID";
                drpDic.DataBind();
                drpDic.Items.Insert(0, new ListItem("--------请选择--------", "-1"));

            }


            Hashtable ht = sysSetingManager.GetItemByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                this.hidId.Value = ht["ID"].ToString();
                this.txtName.Text = ht["NAME"].ToString();
                var settingKey = ht["SETTINGKEY"].ToString();
                if (settingKey.Contains("Image"))
                {

                    this.PanelImg.Visible = true;
                    this.PanelTxt.Visible = false;
                    var htmlImag = "<img  src=\"" + ht["SETTINGCONTENT"].ToString() + "\" />";
                    //// $("#hidPath").val(response);
                    this.uploadImg.InnerHtml = htmlImag;
                    this.hidPath.Value = ht["SETTINGCONTENT"].ToString();
                }
                else
                {
                    this.PanelImg.Visible = false;
                    this.PanelTxt.Visible = true;
                    this.txtSettingContent.Text = ht["SETTINGCONTENT"].ToString();
                }
                this.drpDic.SelectedValue = ht["SETTINGTYPE"].ToString();
                this.txtDescription.Text = ht["DESCRIPTION"].ToString();
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                Hashtable ht = new Hashtable();

                // 条目名称
                ht["Name"] = this.txtName.Text.Trim();
                // 设置的值
                if (this.PanelTxt.Visible)
                {
                    ht["SettingContent"] = this.txtSettingContent.Text.Trim();
                }
                else if (this.PanelImg.Visible)
                {
                    ht["SettingContent"] = this.hidPath.Value;
                }
                //说明
                ht["Description"] = this.txtDescription.Text.Trim();
                ht["SettingType"] = this.drpDic.SelectedValue.ToString();
                Hashtable htSetting=sysSetingManager.GetSettingByID(_key);
                bool result = sysSetingManager.AddOrEdit(ht, _key);
                if (result)
                {
                    Logger.Info("系统参数配置，" + RequestSession.GetSessionUser().UserName + "将【" + htSetting["NAME"].ToString()+"--"+ htSetting["SETTINGCONTENT"].ToString() + "】,修改为【" + ht["Name"].ToString()+"--"+ht["SettingContent"].ToString()+"】");
                    ShowMsgHelper.AlertMsgNoFresh("操作成功！");
                }
                else
                {
                    ShowMsgHelper.Alert_Error("操作失败！");
                }


            }
            catch (Exception ex)
            {
                Logger.Error("系统参数配置:" + ex.Message.ToString());
                throw ex;
            }
        }

    }
}