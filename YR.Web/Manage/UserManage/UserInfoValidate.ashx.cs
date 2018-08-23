using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using YR.Busines;
using System.Collections;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.UserManage
{
    /// <summary>
    /// UserInfo 的摘要说明
    /// </summary>
    public class UserInfoValidate : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            string Action = context.Request["action"];                      //提交动作
            string user_ID = context.Request["user_ID"];                    //用户主键
            Hashtable ht = new Hashtable();
            int Return = -1;
            switch (Action)
            {
                case "accredit":                                            //用户信息启用
                    ht["DeleteMark"] = 1;
                    Return = DataFactory.SqlDataBase().UpdateByHashtable("YR_UserInfo", "ID", user_ID, ht);
                    context.Response.Write(Return.ToString());
                    break;
                case "lock":                                                //锁定用户信息
                    ht["UserState"] = 0;
                    Return = DataFactory.SqlDataBase().UpdateByHashtable("YR_UserInfo", "ID", user_ID, ht);
                    context.Response.Write(Return.ToString());
                    break;
                case "abnormal":
                    string type="1";
                    if (!string.IsNullOrEmpty(context.Request["type"]))
                        type = context.Request["type"];
                    string ordernum = context.Request["ordernum"];
                    //异常还车扣费类型,type:1收费,2不收费
                    Return = AbnormalProcess(ordernum, type);
                    context.Response.Write(Return.ToString());
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 异常还车
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="type">费用支付，1收费，2免费</param>
        /// <returns></returns>
        private int AbnormalProcess(string ordernum, string type)
        {
            UserInfoManager userManager = new UserInfoManager();
            bool result = false;
            result = userManager.AbnormalReturnVehicle(ordernum, type == "1" ? true : false);
            return result ? 1 : 0;
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}