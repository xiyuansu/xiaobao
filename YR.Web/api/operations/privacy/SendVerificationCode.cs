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
    /// 发送验证码
    /// </summary>
    public class SendVerificationCode : IApiAction2
    {
        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["Mobile"] == null || res["Mobile"].ToString().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "手机号码为空");
            }
            else
            {
                string mobile = res["Mobile"].ToString().Trim();
                SmsInfoManager sim = new SmsInfoManager();
                OPUserManager userManager = new OPUserManager();
                DataTable user = userManager.GetUserInfoByMobile(mobile);
                //判断用户是否在系统中进行过注册
                if (user == null || user.Rows.Count == 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "用户未在系统注册,无法收取验证码");
                }
                if (user != null)
                {
                    if (user.Rows.Count > 0)
                    {
                        Hashtable hashuser = DataTableHelper.DataRowToHashTable(user.Rows[0]);
                        //判断用户是否禁用状态
                        if (SiteHelper.GetHashTableValueByKey(hashuser, "UserState").Trim() == UserState.Disable.GetHashCode().ToString())
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "用户被禁用,无法收取验证码");
                        }
                    }
                }

                bool sendResult = sim.SendVerificationCode(mobile);
                if (sendResult)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "短信发送成功");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "短信发送失败");
                }
            }
        }
    }
}