using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using YR.Common.DotNetCache;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 发送验证码
    /// </summary>
    public class SendCode : IApiAction2
    {
        private static Log Logger = LogFactory.GetLogger(typeof(SendVerificationCode));
        //每个手机号每天最多发送验证码次数
        private const int MaxSendTimes = 5;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["Mobile"] == null || res["Mobile"].ToString().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "手机号码为空");
            }
            if (res["LoginCode"] == null || res["LoginCode"].ToString().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "图形码为空");
            }
            string mobile = res["Mobile"].ToString().Trim();
            UserInfoManager uim = new UserInfoManager();
            DataTable blackMobile = uim.GetBlackMobileByMobile(mobile);

            if (blackMobile != null&&blackMobile.Rows.Count>0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "手机号被禁用,请联系客服");
            }
            string code = res["LoginCode"].ToString().Trim();
            code = code.ToLower();
            ICache cache = CacheFactory.GetCache();
            string loginCodeKey = "login_code_" + mobile;
            string code_value = cache.Get<string>(loginCodeKey);
            Logger.Debug("mobile:" + mobile + ",code:" + code + ",redis_code_value:" + code_value);
            /*if(string.IsNullOrEmpty(code_value))
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "图形码超时");
            }
            if (!code_value.Equals(code))
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "图形码错误");
            }*/

            if (!ValidateUtil.IsMobilePhone(mobile))
            {
                if (cache != null)
                {
                    cache.Dispose();
                }
                return SiteHelper.GetJsonFromHashTable(null, "faild", "手机格式不正确");
            }

            SmsInfoManager sim = new SmsInfoManager();
            //判断用户是否禁用状态
            DataTable user = uim.GetUserInfoByBindPhone(mobile);
            if (user != null)
            {
                if (user.Rows.Count > 0)
                {
                    Hashtable hashuser = DataTableHelper.DataRowToHashTable(user.Rows[0]);
                    if (SiteHelper.GetHashTableValueByKey(hashuser, "UserState").Trim() == UserState.Disable.GetHashCode().ToString())
                    {
                        if (cache != null)
                        {
                            cache.Dispose();
                        }
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "用户被禁用,无法收取验证码");
                    }
                }
            }

            /*string span_key_id = "sms_send_time_span@" + mobile;
            if (string.IsNullOrEmpty(Cache.Get<string>(span_key_id)))
            {
                DateTime dt = DateTime.Now.AddSeconds(60);
                Cache.Set(span_key_id, mobile, dt - DateTime.Now);
            }
            else
            {
                Cache.Dispose();
                Logger.Error("发送短信,Mobile:" + mobile + "60秒内");
                return SiteHelper.GetJsonFromHashTable(null, "faild", "短信发送失败,请稍后再试");
            }*/

            string keyid = "sms_send_times@" + mobile;
            int sms_send_times = 0;
            if (string.IsNullOrEmpty(cache.Get<string>(keyid)))
            {
                DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                cache.Set(keyid, sms_send_times, dt - DateTime.Now);
            }
            else
            {
                string tims = cache.Get<string>(keyid);
                int.TryParse(tims, out sms_send_times);
            }
            if (sms_send_times >= MaxSendTimes)
            {
                if (cache != null)
                {
                    cache.Dispose();
                }
                return SiteHelper.GetJsonFromHashTable(null, "faild", "短信发送失败,短信发送已超过当天最大次数");
            }

            bool sendResult = sim.SendVerificationCode(mobile);
            if (sendResult)
            {
                sms_send_times += 1;
                DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                cache.Set(keyid, sms_send_times, dt - DateTime.Now);
                cache.Dispose();
                return SiteHelper.GetJsonFromHashTable(null, "success", "短信发送成功");
            }
            else
            {
                if (cache != null)
                {
                    cache.Dispose();
                }
                return SiteHelper.GetJsonFromHashTable(null, "faild", "短信发送失败");
            }
        }
    }
}