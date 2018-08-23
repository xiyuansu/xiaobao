using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetUI;
using YR.Web.App_Code;

namespace YR.Web.Manage.UserManage
{
    public partial class OperateUser_Form : PageBase
    {
        private OPUserManager userManager = new OPUserManager();

        private string _key
        {
            get
            {
                return Request["key"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.BindParkList();
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
            Hashtable ht = userManager.GetUserInfoByUserID(_key);
            if (ht != null && ht.Keys.Count>0)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
                
                DataTable list = userManager.GetUserParkingList(_key);
                foreach(DataRow dr in list.Rows)
                {
                    ParkingList.Items.FindByValue(dr["ID"].ToString()).Selected = true;
                }

            }
        }

        private void BindParkList()
        {
            ServiceAreaManager areaManager = new ServiceAreaManager();
            DataTable parkingList = areaManager.GetAllParkings(1, 9999);
            ParkingList.DataSource = parkingList;
            ParkingList.DataTextField = "ThisName";
            ParkingList.DataValueField = "ID";
            ParkingList.DataBind();
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
                ht["UserName"] = UserName.Value;
                ht["NickName"] = NickName.Value;
                ht["UserSex"] = UserSex.Value;
                ht["ReceiveSMS"] = ReceiveSMS.Value;
                ht["Tel"] = Tel.Value;
                ht["Email"] = Email.Value;
                ht["IDCardNum"] = IDCardNum.Value;
                ht["UserState"] = 1;
                if (!string.IsNullOrEmpty(_key))
                {
                    ht["UserID"] = _key;
                }
                else
                {
                    ht["UserID"] = CommonHelper.GetGuid;
                    ht["DeleteMark"] = "1";
                    ht["Createtime"] = DateTime.Now;
                }

                bool returnValue = userManager.AddOrEditUserInfo(ht, _key);

                List<string> list = new List<string>();
                foreach (ListItem item in ParkingList.Items)
                {
                    if (item.Selected)
                        list.Add(item.Value);
                }
                returnValue = userManager.SaveUserParkingList(ht["UserID"].ToString(), list);

                if (returnValue)
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

        #endregion

    }
}