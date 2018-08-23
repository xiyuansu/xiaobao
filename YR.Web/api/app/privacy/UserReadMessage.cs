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
    /// 用户阅读消息
    /// </summary>
    public class UserReadMessage : IApiAction2
    {
        private string uid = string.Empty;

        private string messageid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["MessageID"] == null || res["MessageID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                messageid = res["MessageID"].ToString().Trim();
                Hashtable userreadmessage = new Hashtable();
                userreadmessage["ID"] = CommonHelper.GetGuid;
                userreadmessage["UserID"] = uid;
                userreadmessage["MessageID"] = messageid;
                userreadmessage["IsRead"] = 1;
                userreadmessage["ReadTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                MessageReadLogManager mrlm = new MessageReadLogManager();
                int num = 0;
                if (!mrlm.IsUserReadMessages(uid, messageid))
                    num = mrlm.AddMessageReadLog(userreadmessage);
                else
                    num = 1;
                if (num > 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "消息已读标记成功");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "消息已读标记失败");
                }
            }
        }

    }
}