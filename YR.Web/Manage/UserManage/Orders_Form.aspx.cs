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
    public partial class Orders_Form : PageBase
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
                if (!string.IsNullOrEmpty(Request.QueryString["Picurl"]))
                {
                    ImageDrag.ImageUrl = Request.QueryString["Picurl"];
                    ImageIcon.ImageUrl = Request.QueryString["Picurl"];
                    Page.ClientScript.RegisterStartupScript(typeof(User_Form), "step2", "<script type='text/javascript'>Step2();</script>");
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(typeof(User_Form), "step1", "<script type='text/javascript'>Step1();</script>");
                }
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

        #region 上传头像

        private const string savepath = "User/UserHeadImage" + "/";

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuPhoto.PostedFile != null && fuPhoto.PostedFile.ContentLength > 0)
            {
                string ext = System.IO.Path.GetExtension(fuPhoto.PostedFile.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jepg" && ext != ".bmp" && ext != ".gif")
                {
                    return;
                }
                string filename = "yuantu_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string path = "~/UploadPhoto/" + filename;
                fuPhoto.PostedFile.SaveAs(Server.MapPath(path));

                ImageDrag.ImageUrl = Request.QueryString["Picurl"];
                ImageIcon.ImageUrl = Request.QueryString["Picurl"];
                Page.ClientScript.RegisterStartupScript(typeof(User_Form), "step2", "<script type='text/javascript'>Step2();</script>");
                //Response.Redirect("~/UpLoadUserPhoto.aspx?Picurl=" + Server.UrlEncode(path));
            }
            else
            {
                Response.Write("没有任何文件");
            }
        }
        protected void btn_Image_Click(object sender, EventArgs e)
        {
            int imageWidth = Int32.Parse(txt_width.Text);
            int imageHeight = Int32.Parse(txt_height.Text);
            int cutTop = Int32.Parse(txt_top.Text);
            int cutLeft = Int32.Parse(txt_left.Text);
            int dropWidth = Int32.Parse(txt_DropWidth.Text);
            int dropHeight = Int32.Parse(txt_DropHeight.Text);

            //string filename = CutPhotoHelp.SaveCutPic(Server.MapPath(ImageIcon.ImageUrl), Server.MapPath(savepath), 0, 0, dropWidth,
            //                        dropHeight, cutLeft, cutTop, imageWidth, imageHeight);
            //this.imgphoto.ImageUrl = savepath + filename;
            //Page.ClientScript.RegisterStartupScript(typeof(User_Form), "step3", "<script type='text/javascript'>Step3();</script>");
        }

        #endregion
    }
}