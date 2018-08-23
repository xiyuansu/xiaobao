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
    /// 获取用户消息列表
    /// </summary>
    public class GetUserMessages : IApiAction2
    {
        private string uid = string.Empty;

        private string messagetype = string.Empty;

        private int currentpage = 1;

        private int pagenum = 10;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"] == null || res["CurrentPage"].ToString().Trim().Length <= 0 ||
                res["PageNum"] == null || res["PageNum"].ToString().Trim().Length <= 0)
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
                    pagenum = int.Parse(res["PageNum"].ToString().Trim());
                    currentpage = int.Parse(res["CurrentPage"].ToString().Trim());
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }
                MessageManager mm = new MessageManager();
                DataTable messages = mm.GetMessageInfoPageByUser(uid, messagetype, currentpage, pagenum);
                if (messages == null || messages.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "没有数据了");
                }
                else
                {
                    messages.Columns.Add("IsRead");
                    foreach (DataRow message in messages.Rows)
                    {
                        string messageid = message["ID"].ToString();
                        if (message["ImageUrl"].ToString().Trim().Length > 0)
                        {
                            message["ImageUrl"] = SiteHelper.GetFileServer() + message["ImageUrl"].ToString().Trim();
                        }
                        message["MessageUrl"] = string.Format("{0}/Manage/InformationManage/Notification.aspx?id={1}", SiteHelper.GetFileServer(), message["ID"].ToString());
                        message["IsRead"] = "0";
                        MessageReadLogManager mrlm = new MessageReadLogManager();
                        if (!mrlm.IsUserReadMessages(uid, messageid))
                        {
                            //系统消息设置为已读
                            if (message["MessageType"].ToString() == "1")
                            {
                                Hashtable userreadmessage = new Hashtable();
                                userreadmessage["ID"] = CommonHelper.GetGuid;
                                userreadmessage["UserID"] = uid;
                                userreadmessage["MessageID"] = messageid;
                                userreadmessage["IsRead"] = 1;
                                userreadmessage["ReadTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                                mrlm.AddMessageReadLog(userreadmessage);

                                message["IsRead"] = "1";
                            }
                        }
                        else
                        {
                            message["IsRead"] = "1";
                        }
                        if (message["MessageType"].ToString() == "2")
                            message["MessageContent"] = "";

                        message["Summary"] = message["Summary"].ToString().Replace("\r\n", "");
                    }
                    return JsonHelper.DataTableToJson("success", "获取数据成功", messages, "Messages");
                }
            }
        }
    }
}