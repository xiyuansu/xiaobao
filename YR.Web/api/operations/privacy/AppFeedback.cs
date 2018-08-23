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
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// App反馈意见
    /// </summary>
    public class AppFeedback : IApiAction2
    {
        private string uid = string.Empty;
        private string content = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["Content"] == null
                || res["UID"].ToString().Trim().Length <= 0 || res["Content"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    content = res["Content"].ToString().Trim();
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }

                string userName = "";
                OPUserManager hum = new OPUserManager();
                Hashtable user_ht = hum.GetUserInfoByUserID(uid);
                if (user_ht != null && user_ht.Count > 0)
                    userName = user_ht["USERNAME"].ToString();

                Hashtable ht = new Hashtable();
                ht["ID"] = CommonHelper.GetGuid;
                ht["Content"] = content;
                ht["UserID"] = uid;
                ht["UserName"] = userName;
                ht["AppID"] = "002";
                ht["AppName"] = "运维app";
                ht["ReadFlag"] = 0;
                FeedbackManager fm = new FeedbackManager();
                bool isSuccess = fm.AddFeedback(ht);
                if (isSuccess)
                {

                    return SiteHelper.GetJsonFromHashTable(null, "success", "意见反馈成功");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "意见反馈失败");
                }

            }
        }
    }
}