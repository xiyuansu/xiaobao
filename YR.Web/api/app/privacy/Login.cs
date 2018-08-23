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
    /// 登录
    /// </summary>
    public class Login : IApiAction2
    {
        string mobile = string.Empty;
        string verificationCode = string.Empty;
        string client = string.Empty;
        string longitude = string.Empty;
        string latitude = string.Empty;
        string drviceid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["Mobile"] == null ||
                res["VerificationCode"] == null ||
                res["Client"] == null ||
                res["Longitude"] == null ||
                res["Latitude"] == null ||
                res["EquipmentID"] == null ||
                res["Mobile"].ToString().Trim().Length <= 0 ||
                res["VerificationCode"].ToString().Trim().Length <= 0 ||
                res["Client"].ToString().Trim().Length <= 0 ||
                res["Longitude"].ToString().Trim().Length <= 0 ||
                res["Latitude"].ToString().Trim().Length <= 0 ||
                res["EquipmentID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "登录数据不完整");
            }
            else
            {
                UserInfoManager uim = new UserInfoManager();
                SmsInfoManager sim = new SmsInfoManager();
                UserLoginLogManager ullm = new UserLoginLogManager();
                SysSettingManager settingManager = new SysSettingManager();
                bool isAllowLogin = false;
                //获取参数
                mobile = res["Mobile"].ToString().Trim();
                verificationCode = res["VerificationCode"].ToString().Trim();
                client = res["Client"].ToString().Trim();
                longitude = res["Longitude"].ToString().Trim();
                latitude = res["Latitude"].ToString().Trim();
                drviceid = res["EquipmentID"].ToString().Trim();
                DataTable user = uim.GetUserInfoByBindPhone(mobile);
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
                else
                {
                    Hashtable newuser = new Hashtable();
                    newuser["BindPhone"] = mobile;
                    newuser["RealNameCertification"] = (int)UserRealNameCertification.Unauthorized;
                    newuser["UserState"] = (int)UserState.Enable;
                    newuser["Balance"] = 0;
                    newuser["RaiseBalance"] = 0;
                    newuser["IsRaiseUser"] = (int)UserIsRaise.NO;
                    newuser["InviteCode"] = mobile;
                    int num = uim.AddUserInfo(newuser);
                    if (num > -1)
                    {
                        user = uim.GetUserInfoByBindPhone(mobile);
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
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "手机号错误");
                    }

                    //新注册用户赠送优惠券
                    int couponNum = 0;
                    int.TryParse( settingManager.GetValueByKey("RegCouponNum"),out couponNum);
                    decimal couponMoney = 0.00m;
                    decimal.TryParse(settingManager.GetValueByKey("RegCouponMoney"), out couponMoney);
                    UserCouponManager couponManager = new UserCouponManager();
                    if(couponNum>0 && couponMoney>0)
                    {
                        couponManager.NewUserGiveCoupons(user.Rows[0]["ID"].ToString());
                    }
                }
                if (isAllowLogin)
                {
                    //记录登录日志
                    Hashtable loginlog = new Hashtable();
                    loginlog["UserID"] = user.Rows[0]["ID"].ToString();
                    loginlog["Latitude"] = latitude;
                    loginlog["Longitude"] = longitude;
                    loginlog["Client"] = client;
                    ullm.AddUserLoginLog(loginlog);
                    //更新最后登录时间
                    Hashtable userh = new Hashtable();
                    userh["LastloginTime"] = SiteHelper.GetWebServerCurrentTime();
                    userh["DeviceID"] = drviceid;
                    uim.AddOrEditUserInfo(userh, loginlog["UserID"].ToString());

                    decimal balance=0.00m;
                    decimal.TryParse(user.Rows[0]["Balance"].ToString(),out balance);
                    decimal balance2=0.00m;
                    decimal.TryParse(user.Rows[0]["Balance2"].ToString(),out balance2);
                    user.Rows[0]["Balance"] = balance + balance2;
                    if (user.Rows[0]["UserType"].ToString() ==UserType.OffDepositUser.GetHashCode().ToString())//免押金用户
                    {
                        string depositMoney = settingManager.GetValueByKey("GoldDepositAmount");
                        user.Rows[0]["Deposit"] = depositMoney;
                    }

                    //响应数据到客户端
                    return SiteHelper.GetJsonFromHashTable(DataTableHelper.DataRowToHashTable(user.Rows[0]), "success", "登录成功", "UserInfo");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "登录操作失败");
                }
            }
        }

    }
}