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
using Asiasofti.SmartVehicle.Common;

namespace YR.Web.Manage.InformationManage
{
    public partial class Notification_Form : System.Web.UI.Page
    {
        MessageManager messageManager = new MessageManager();
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
            Hashtable ht = messageManager.GetMessageInfoByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                this.txtMessageTitle.Text = ht["MESSAGETITLE"].ToString();
                this.txtBegin.Text = ht["BEGINTIME"].ToString();
                this.txtEnd.Text = ht["ENDTIME"].ToString();
                this.txtSummary.Text = ht["SUMMARY"].ToString();
                this.ckeId.Text = ht["MESSAGECONTENT"].ToString();
                if (ht["IMAGEURL"].ToString().Length > 0)
                {
                    var htmlImag = "<img ID=\"Image1\" src=\""+ht["IMAGEURL"].ToString() + "\" />";
                    this.hidPath.Value = ht["IMAGEURL"].ToString();
                    this.uploadImg.InnerHtml = htmlImag;
                }
                else
                {
                    this.hidPath.Value = "";
                    this.uploadImg.InnerHtml = "";
                }
                if (ht["STATE"].ToString().Equals("1"))
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
            try
            {

                Hashtable ht = new Hashtable();
                // 消息标题
                ht["MessageTitle"] = this.txtMessageTitle.Text.Trim();
                // 启用时间
                ht["BeginTime"] = txtBegin.Text.Trim();
                // 结束时间
                ht["EndTime"] = txtEnd.Text.Trim();
                // 摘要简介
                ht["Summary"] = this.txtSummary.Text.Trim();
                // 消息内容
                ht["MessageContent"] = this.ckeId.Text;
                // 图片地址
                ht["ImageUrl"] = this.hidPath.Value.ToString().Trim();
                //消息类型 2 用户消息
                ht["MessageType"] = "2";
                // 状态（0未启用，1启用）
                if (radOK.Checked)
                {
                    ht["State"] = "1";
                }
                else
                {
                    ht["State"] = "0";
                }
                // 操作人员ID
                ht["OperatorID"] = RequestSession.GetSessionUser().UserId.ToString();
                //发布时间
                ht["ReleaseTime"] = DateTime.Now.ToString();
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                }
                bool result = messageManager.AddOrEditMessageInfo(ht, _key);

                if (result)
                {
                    ShowMsgHelper.AlertMsg("操作成功！");
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
            return SiteHelper.GetAppsetString("UploadImagePath")+DateTime.Now.ToString("yyyyMMdd")+"/";
        }
    }
}