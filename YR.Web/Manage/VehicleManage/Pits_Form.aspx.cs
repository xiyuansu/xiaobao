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
    public partial class Pits_Form : PageBase
    {
        private string _key
        {
            get
            {
                return Request["key"];
            }
        }
        public DataTable PitsImageTable
        {
            get
            {
                SystemImagesManager imagesManager = new SystemImagesManager();
                return imagesManager.GetImagesByPitsID(this._key);
            }
        }
        private YR_UserInfo_Dal userInfoDal = new YR_UserInfo_Dal();
        private PitsManager pitsManager = new PitsManager();
        public StringBuilder strUserHtml = new StringBuilder();//所属角色下的用户
        protected void Page_Load(object sender, EventArgs e)
        {
            //CreateUserName.Value = RequestSession.GetSessionUser().UserName.ToString();
            //CreateDate.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (!IsPostBack)
            {
                // 绑定用户
                this.InitInfoOrg();
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
                this.tab1.Visible = true;
                this.tab2.Visible = true;
                ControlBindHelper.SetWebControls(this.Page, ht);
            }
        }
        /// <summary>
        /// 所属部门
        /// </summary>
        public void InitInfoOrg()
        {
            string deptNames = SiteHelper.GetAppsetString("DataAuthDept");
            DataTable dtOrg = userInfoDal.GetOrganizeList(deptNames);
            if (DataTableHelper.IsExistRows(dtOrg))
            {
                DataView dv = new DataView(dtOrg);
                //dv.RowFilter = "ParentId = '0'";
                foreach (DataRowView drv in dv)
                {
                    strUserHtml.Append("<li>");
                    strUserHtml.Append("<div>" + drv["Organization_Name"].ToString() + "</div>");
                    //创建子节点
                    strUserHtml.Append(GetTreeNodeOrg(drv["Organization_ID"].ToString()));
                    strUserHtml.Append("</li>");
                }
            }
            else
            {
                strUserHtml.Append("<li>");
                strUserHtml.Append("<div><span style='color:red;'>暂无数据</span></div>");
                strUserHtml.Append("</li>");
            }
        }
        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="parentID">父节点主键</param>
        /// <param name="dtMenu"></param>
        /// <returns></returns>
        public string GetTreeNodeOrg(string parentID)
        {
            StringBuilder sb_TreeNode = new StringBuilder();
            DataTable dtNode = userInfoDal.GetUserInfoByOrgID(parentID);
            DataView dv = new DataView(dtNode);
            //dv.RowFilter = "ParentId = '" + parentID + "'";
            if (dv.Count > 0)
            {
                sb_TreeNode.Append("<ul>");
                DataTable dtPitsUser = pitsManager.GetPitsUserInfoByID(_key);
                string checkStr="";
                DataRow[] dr = null;
                foreach (DataRowView drv in dv)
                {
                    dr=dtPitsUser.Select("UserID='" + drv["User_ID"].ToString() + "'");
                    checkStr = dr != null && dr.Length > 0 ? "checked=\"checked\"" : "";
                    sb_TreeNode.Append("<li>");
                    sb_TreeNode.Append("<div class='treeview-file'>");
                    sb_TreeNode.Append("<input style='vertical-align: middle;margin-bottom:2px;' type=\"checkbox\" " + checkStr + " value=\"" + drv["User_ID"] + "\" name=\"chkUserID\" />");
                    sb_TreeNode.Append(drv["User_Name"].ToString() + "</div>");
                    //创建子节点
                    //sb_TreeNode.Append(GetTreeNodeOrg(drv["Organization_ID"].ToString(), dtNode, dtStaffOrganize));
                    sb_TreeNode.Append("</li>");
                }
                sb_TreeNode.Append("</ul>");
            }
            return sb_TreeNode.ToString();
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

                // 维修站维修人员
                string userIDs = Request["chkUserID"];

                bool returnValue = false;
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                    returnValue = pitsManager.EditPitsInfo(ht, _key, userIDs != null ? userIDs.Split(',') : null);
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                    ht["CreateTime"] = DateTime.Now;
                    ht["DeleteMark"] = "0";
                    returnValue = pitsManager.AddPitsInfo(ht, _key, userIDs != null ? userIDs.Split(',') : null);
                }
                #endregion

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