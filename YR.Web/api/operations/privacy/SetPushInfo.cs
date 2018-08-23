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

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 设置第三方消息推送标识ID
    /// </summary>
    public class SetPushInfo : IApiAction2
    {
        private string uid = string.Empty;
        private string pushid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["PushID"] == null || res["PushID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                pushid = res["PushID"].ToString().Trim();

                OPUserManager userManager = new OPUserManager();
                Hashtable user = userManager.GetUserInfoByUserID(uid);
                if (user == null || user.Keys.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "用户不存在");
                }
                else
                {
                    Hashtable modifyUser = new Hashtable();
                    modifyUser["PushID"] = pushid;
                    bool issuccess = userManager.AddOrEditUserInfo(modifyUser, uid);
                    if (!issuccess)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "保存失败");
                    }
                }

                return SiteHelper.GetJsonFromHashTable(null, "success", "保存成功", "SetPushInfo");
            }
        }
    }
}