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

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 得到未读消息个数
    /// </summary>
    public class GetNoReadMessageCount : IApiAction2
    {
        private string uid = string.Empty;

        private string messagetype = "1";

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    if (res["MessageType"] != null && res["MessageType"].ToString().Trim().Length > 0)
                    {
                        //消息类型,1:系统消息，2:公告、通知
                        messagetype = res["MessageType"].ToString().Trim();
                    }
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "参数格式不正确");
                }
                MessageManager mm = new MessageManager();
                string count = mm.GetNoReadMessagesCountByUser(uid,messagetype);
                Hashtable hashcount = new Hashtable();
                hashcount["COUNT"] = count;
                return SiteHelper.GetJsonFromHashTable(hashcount, "success", "获取数据成功");
            }
        }
    }
}