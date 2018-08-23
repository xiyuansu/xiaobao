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
using YR.Common.DotNetCache;
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
    /// 发送验证码
    /// </summary>
    public class SendVerificationCode : IApiAction2
    {
        //每个手机号每天最多发送验证码次数
        private const int MaxSendTimes = 10;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["Mobile"] == null || res["Mobile"].ToString().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "手机号码为空");
            }
            else
            {
                string mobie = res["Mobile"].ToString().Trim();
                SmsInfoManager sim = new SmsInfoManager();
                UserInfoManager uim = new UserInfoManager();
                //判断用户是否禁用状态
                DataTable user = uim.GetUserInfoByBindPhone(mobie);
                if (user != null)
                {
                    if (user.Rows.Count > 0)
                    {
                        Hashtable hashuser = DataTableHelper.DataRowToHashTable(user.Rows[0]);
                        if (SiteHelper.GetHashTableValueByKey(hashuser, "UserState").Trim() == UserState.Disable.GetHashCode().ToString())
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "用户被禁用,无法收取验证码");
                        }
                    }
                }

                ICache Cache = CacheFactory.GetCache();
                string keyid = "sms_send_times@" + mobie;
                int sms_send_times=0;
                if (string.IsNullOrEmpty(Cache.Get<string>(keyid)))
                {
                    DateTime dt=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,23,59,59);
                    Cache.Set(keyid, sms_send_times, dt-DateTime.Now);
                }
                if (sms_send_times >= MaxSendTimes)
                {
                    Cache.Dispose();
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "短信发送失败,短信发送已超过当天最大次数");
                }

                bool sendResult = sim.SendVerificationCode(mobie);
                if (sendResult)
                {
                    sms_send_times += 1;
                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Cache.Set(keyid, sms_send_times, dt - DateTime.Now);
                    Cache.Dispose();
                    return SiteHelper.GetJsonFromHashTable(null, "success", "短信发送成功");
                }
                else
                {
                    Cache.Dispose();
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "短信发送失败");
                }
            }
        }

    }
}