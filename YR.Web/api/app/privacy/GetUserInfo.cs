using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
    /// 获取用户信息
    /// </summary>
    public class GetUserInfo : IApiAction2
    {
        private string uid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                UserInfoManager uim = new UserInfoManager();
                DataTable user = uim.GetUserInfoByUid(uid);
                if (user == null || user.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到用户信息");
                }
                else
                {
                    decimal balance = 0.00m;
                    decimal.TryParse(user.Rows[0]["Balance"].ToString(), out balance);
                    decimal balance2 = 0.00m;
                    decimal.TryParse(user.Rows[0]["Balance2"].ToString(), out balance2);
                    user.Rows[0]["Balance"] = balance + balance2;
                    if(user.Rows[0]["UserType"].ToString() ==UserType.OffDepositUser.GetHashCode().ToString())//免押金用户
                    {
                        SysSettingManager settingManager = new SysSettingManager();
                        string depositMoney = settingManager.GetValueByKey("GoldDepositAmount");
                        user.Rows[0]["Deposit"] = depositMoney;
                    }

                    Hashtable result = DataTableHelper.DataRowToHashTable(user.Rows[0]);
                    return SiteHelper.GetJsonFromHashTable(result, "success", "获取数据成功", "UserInfo");
                }
            }
        }
    }
}