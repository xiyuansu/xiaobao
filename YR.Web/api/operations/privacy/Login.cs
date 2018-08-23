using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System.Collections;
using System.Data;
using YR.Common.DotNetData;
using YR.Web.api.api_class;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 登录
    /// </summary>
    public class Login : IApiAction2
    {
        string mobile = string.Empty;
        string verificationCode = string.Empty;
        string client = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["Mobile"] == null ||
                res["VerificationCode"] == null ||
                res["Client"] == null ||
                res["Mobile"].ToString().Trim().Length <= 0 ||
                res["VerificationCode"].ToString().Trim().Length <= 0 || res["Client"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "登录数据不完整");
            }
            else
            {
                OPUserManager userManager = new OPUserManager();
                SmsInfoManager sim = new SmsInfoManager();
                bool isAllowLogin = false;
                //获取参数
                mobile = res["Mobile"].ToString().Trim();
                verificationCode = res["VerificationCode"].ToString().Trim();
                client = res["Client"].ToString().Trim();
                DataTable user = userManager.GetUserInfoByMobile(mobile);
                //判断用户是否在系统中进行过注册
                if (user == null || user.Rows.Count == 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "用户未在系统注册,登录失败");
                }
                if (user != null && user.Rows.Count > 0)
                {
                    string vcode = sim.GetVerCodeByMobile(mobile);
                    if (vcode != null && verificationCode.Trim() == vcode.Trim())
                    {
                        isAllowLogin = true;
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "验证码不正确");
                    }
                }
                if (isAllowLogin)
                {
                    //更新最后登录时间
                    Hashtable userh = new Hashtable();
                    userh["LastloginTime"] = SiteHelper.GetWebServerCurrentTime();
                    userManager.AddOrEditUserInfo(userh, user.Rows[0]["UserID"].ToString());

                    //响应数据到客户端
                    Hashtable huser = DataTableHelper.DataTableToHashtable(user);
                    DataTable dt_parking = userManager.GetUserParkingList(huser["USERID"].ToString());
                    DataRow dr = dt_parking.NewRow();
                    dr["ID"] = System.Guid.Empty.ToString();
                    dr["ThisName"] = "停车点外";
                    dt_parking.Rows.InsertAt(dr, 0);
                    huser["PARKLIST"] = dt_parking;
                    //huser["PARKLIST"] = userManager.GetUserParkingList(huser["USERID"].ToString());
                    return SiteHelper.GetJsonFromHashTable2(huser, "success", "登录成功", "UserInfo");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "登录失败,请稍后重试");
                }
            }
        }
    }
}