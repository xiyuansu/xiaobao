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
    /// 实名认证
    /// </summary>
    public class IdentityVerification : IApiAction2
    {
        private string uid = string.Empty;

        private string realName = string.Empty;

        private string idCard = string.Empty;

        //每个用户每天最多认证次数
        private const int MaxVerfyTimes = 5;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["RealName"] == null || res["RealName"].ToString().Trim().Length <= 0 ||
                res["IDCard"] == null || res["IDCard"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    if (res["RealName"] != null)
                        realName = res["RealName"].ToString().Trim();
                    if (res["IDCard"] != null)
                        idCard = res["IDCard"].ToString().Trim().ToLower();
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }

                if (!CheckIDCard18(idCard))
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "身份证号错误");
                }

                UserInfoManager uim = new UserInfoManager();

                Hashtable user = uim.GetUserInfoByUserID(uid);
                int realNameCertification = 1;
                int.TryParse(SiteHelper.GetHashTableValueByKey(user, "RealNameCertification"), out realNameCertification);
                if (realNameCertification == 4)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您已经通过实名认证");
                }

                //验证身份证号，判断是否注册人员已满16岁或超过80岁
                if (idCard.Length == 18)
                {
                    string birth = idCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                    DateTime dt = new DateTime();
                    DateTime.TryParse(birth, out dt);
                    if (DateTime.Now.AddYears(-16) < dt)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "实名认证失败,认证用户未满16岁");
                    }
                    if (DateTime.Now.AddYears(-60) >= dt)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "实名认证失败,认证用户已满60岁");
                    }
                }

                ICache Cache = CacheFactory.GetCache();
                string keyid = "id_verfy_times@" + uid;
                int id_verfy_times = 0;
                if (string.IsNullOrEmpty(Cache.Get<string>(keyid)))
                {
                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Cache.Set(keyid, id_verfy_times, dt - DateTime.Now);
                }
                if (id_verfy_times >= MaxVerfyTimes)
                {
                    Cache.Dispose();
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "实名认证失败,认证已超过当天最大次数");
                }

                IDCardVerify verify = new IDCardVerify();
                ObjectApiResp result = new ObjectApiResp();
                result.Code = "0";
                result.Data = "1";

                result = verify.VerifyCard(realName, idCard);
                //result.Code = "1";
                //code：0,访问正常，-1,访问异常，data:1,验证一致,2,不一致,message:文本消息
                if (result.Code == "-1")
                {
                    Cache.Dispose();
                    return SiteHelper.GetJsonFromHashTable(null, "faild", result.Message);
                }
                else
                {
                    id_verfy_times += 1;
                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Cache.Set(keyid, id_verfy_times, dt - DateTime.Now);
                    Cache.Dispose();

                    if (result.Data.ToString() == "1")
                    {
                        bool isUpdate = uim.UpdateRealNameCertification(uid, realName, idCard);
                        if (isUpdate)
                        {
                            DataTable userInfo = uim.GetUserInfoByBindPhone(SiteHelper.GetHashTableValueByKey(user, "BindPhone"));
                            Hashtable result_ht = DataTableHelper.DataRowToHashTable(userInfo.Rows[0]);
                            return SiteHelper.GetJsonFromHashTable(result_ht, "success", "实名认证成功", "UserInfo");
                        }
                        else
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "实名认证状态保存失败");
                        }
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "实名认证没通过，请重试");
                    }
                }
            }
        }

        /// <summary>  
        /// 18位身份证号码验证  
        /// </summary>  
        private bool CheckIDCard18(string idNumber)
        {
            if (idNumber.Length != 18)
            {
                return false;
            }
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }  
    }
}