using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 修改用户信息
    /// </summary>
    public class SetUserInfo : IApiAction2
    {
        private string setType = string.Empty;

        private string content = string.Empty;

        private string uid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["SetType"] == null || res["Content"] == null || res["UID"] == null || res["SetType"].ToString().Trim().Length <= 0 || res["Content"].ToString().Trim().Length <= 0 || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                UserInfoManager uim = new UserInfoManager();
                SmsInfoManager sim = new SmsInfoManager();
                //头像：1   姓名：2   昵称：3
                uid = res["UID"].ToString().Trim();
                setType = res["SetType"].ToString().Trim();
                content = res["Content"].ToString().Trim();
                Hashtable user = uim.GetUserInfoByUserID(uid);
                if (user == null || user.Keys.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "用户不存在");
                }
                else
                {
                    if (setType == "1")
                    {
                        //修改头像
                        try
                        {
                            string imgPath = BitmapHelper.toUpload(content);
                            Hashtable modifyUser = new Hashtable();
                            modifyUser["HeadPortrait"] = imgPath;
                            bool issuccess = uim.AddOrEditUserInfo(modifyUser, uid);
                            if (!issuccess)
                            {
                                return SiteHelper.GetJsonFromHashTable(null, "faild", "用户头像保存失败");
                            }
                            else
                            {
                                user["HeadPortrait"] = imgPath;
                            }
                        }
                        catch (Exception e1)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "用户头像保存失败");
                        }
                    }
                    else if (setType == "3")
                    {
                        //修改昵称
                        Hashtable modifyUser = new Hashtable();
                        modifyUser["NickName"] = content;
                        bool issuccess = uim.AddOrEditUserInfo(modifyUser, uid);
                        if (!issuccess)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "昵称保存失败");
                        }
                        else
                        {
                            user["NickName"] = content;
                        }
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "修改项不合法");
                    }
                }
                DataTable dt = uim.GetUserInfoByBindPhone(SiteHelper.GetHashTableValueByKey(user, "BindPhone"));
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "网络异常");
                }
                else
                {
                    decimal balance = 0.00m;
                    decimal.TryParse(dt.Rows[0]["Balance"].ToString(), out balance);
                    decimal balance2 = 0.00m;
                    decimal.TryParse(dt.Rows[0]["Balance2"].ToString(), out balance2);
                    dt.Rows[0]["Balance"] = balance + balance2;
                    Hashtable huser = DataTableHelper.DataRowToHashTable(dt.Rows[0]);
                    return SiteHelper.GetJsonFromHashTable(huser, "success", "操作成功", "UserInfo");
                }
            }
        }

    }
}