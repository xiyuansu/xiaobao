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

namespace YR.Web.YRBase.SysConfig
{
    public partial class DictForm : PageBase
    {
        DictManager dictManager = new DictManager();
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
            DataTable dtDic = dictManager.GetDict();
            if (dtDic.Rows.Count > 0)
            {
                ////网点马厩绑定
                drpDic.DataSource = dtDic;
                drpDic.DataTextField = "Name";
                drpDic.DataValueField = "ID";
                drpDic.DataBind();
                drpDic.Items.Insert(0, new ListItem("--------请选择--------", "-1"));

            }


            Hashtable ht = dictManager.GetDicItemByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                this.txtName.Text = ht["NAME"].ToString();
                this.txtCityCode.Text = ht["CITYCODE"].ToString();
                this.txtZipCode.Text = ht["ZIPCODE"].ToString();
                this.txtLongitude.Text = ht["LONGITUDE"].ToString();
                this.txtLatitude.Text = ht["LATITUDE"].ToString();
                this.txtRemark.Text = ht["REMARK"].ToString();

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
                this.txtSort.Text = ht["SORT"].ToString();
                this.drpDic.SelectedValue = ht["DICTID"].ToString();

            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                Hashtable ht = new Hashtable();

                bool addError = false;

                if (string.IsNullOrEmpty(_key))
                {
                    DataTable dt = dictManager.GetItem(this.drpDic.SelectedValue.ToString(),this.txtName.Text.Trim().ToString());

                    if (dt.Rows.Count > 0)
                    {
                        addError = true;
                        ShowMsgHelper.Alert_Error("此字典项目已经添加，请重新添加！");
                    }
                }

                // 条目名称
                ht["Name"] = this.txtName.Text.Trim();
                // 备注说明
                ht["Remark"] = this.txtRemark.Text.Trim();
                ht["CityCode"] = this.txtCityCode.Text.Trim();
                ht["ZipCode"] = this.txtZipCode.Text.Trim();

                ht["Longitude"] = this.txtLongitude.Text.Trim();
                ht["Latitude"] = this.txtLatitude.Text.Trim();
                //排列顺序
                ht["Sort"] = string.IsNullOrEmpty(this.txtSort.Text.Trim()) ? "9999" : this.txtSort.Text.Trim();
                // 备注说明
                ht["DictID"] = this.drpDic.SelectedValue.ToString();
                // 状态（0未启用，1启用）
                if (radOK.Checked)
                {
                    ht["DeleteMark"] = "1";
                }
                else
                {
                    ht["DeleteMark"] = "0";
                }

                if (!string.IsNullOrEmpty(_key))
                {
                    ht["ID"] = _key;
                }
                else
                {
                    ht["ID"] = CommonHelper.GetGuid;
                }
                if (addError)
                {
                }
                else
                {
                    bool result = dictManager.AddOrEditMessageInfo(ht, _key);

                    if (result)
                    {
                        ShowMsgHelper.AlertMsgNoFresh("操作成功！");
                    }
                    else
                    {
                        ShowMsgHelper.Alert_Error("操作失败！");
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}