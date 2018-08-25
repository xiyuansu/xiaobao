using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using YR.Common.DotNetCache;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;

namespace YR.Web.Ajax
{
    /// <summary>
    /// AjaxRequst 的摘要说明
    /// </summary>
    public class AjaxRequst : IHttpHandler, IRequiresSessionState
    {
        private static Log Logger = LogFactory.GetLogger(typeof(AjaxRequst));

        public void ProcessRequest(HttpContext context)
        {
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "text/plain";
            string Action = context.Request["action"];
            switch (Action)
            {
                case "GetDistrict":
                    ServiceAreaManager areaManager = new ServiceAreaManager();
                    DataTable dtArea = areaManager.GetAreaList(int.Parse(context.Request["levelType"]), int.Parse(context.Request["parentId"]));
                    context.Response.Write(JsonHelper.DataTableToJson("success", "获取数据成功", dtArea, "ServiceArea"));
                    context.Response.End();
                    break;
                case "GetServiceAreaByID":
                    ServiceAreaManager manager = new ServiceAreaManager();
                    DataTable dt = manager.GetServiceAreaByID(context.Request["key"]);
                    context.Response.Write(JsonHelper.DataTableToJson("success", "获取数据成功", dt, "ServiceArea"));
                    context.Response.End();
                    break;
                case "UpdateServiceAreaCoordinates":
                    ServiceAreaManager sam = new ServiceAreaManager();
                    string key = context.Request["key"];
                    Hashtable ht = sam.GetInfoByID(key);
                    ht["COORDINATES"] = context.Request["Coordinates"];
                    ht["LONGITUDE"] = context.Request["Longitude"];
                    ht["LATITUDE"] = context.Request["Latitude"];
                    string areaType = ht["AREATYPE"].ToString();
                    string status = ht["STATUS"].ToString();
                    string deleteMark = ht["DELETEMARK"].ToString();
                    //AreaType 区域类型 1服务范围 2停车网点
                    //Status 启动状态:0禁用,1启用
                    //DeleteMark 0
                    if ("1".Equals(areaType) && "1".Equals(status) && "True".Equals(deleteMark))
                    {
                        ICache cache = null;
                        try
                        {
                            cache = CacheFactory.GetCache();
                            string cacheKey = "ServiceArea_" + ht["CITYID"].ToString();
                            cache.Set(cacheKey, context.Request["Coordinates"]);

                            List<LatLng> area_pts = new List<LatLng>();
                            string coordinates = cache.Get<string>(cacheKey);
                            if(coordinates!=null&& coordinates.Length>0)
                            foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                area_pts.Add(pt);
                            }

                            cache.Dispose();
                        }
                        catch (Exception e)
                        {
                            if (cache != null)
                            {
                                cache.Dispose();
                            }
                            Logger.Error("更新区域:" + ht["THISNAME"].ToString() + "报错:" + e.Message);
                        }
                    }
                    if (sam.AddOrEditInfo(ht, key))
                    {
                        context.Response.Write(JsonHelper.DataTableToJson("success", "更新成功", null, "UpdateServiceAreaCoordinates"));
                    }
                    else
                    {
                        context.Response.Write(JsonHelper.DataTableToJson("faild", "更新失败", null, "UpdateServiceAreaCoordinates"));
                    }
                    context.Response.End();
                    break;
                case "GetInviteCouponInfo":
                    SysSettingManager settingManager = new SysSettingManager();
                    int couponNum = 0;
                    int.TryParse(settingManager.GetValueByKey("InviteCouponNum"), out couponNum);
                    decimal couponMoney = 0.00m;
                    decimal.TryParse(settingManager.GetValueByKey("InviteCouponMoney"), out couponMoney);
                    double CouponPeriod = 0;
                    double.TryParse(settingManager.GetValueByKey("CouponPeriod"), out CouponPeriod);
                    Hashtable result_ht = new Hashtable();
                    result_ht["CouponNum"] = couponNum;
                    result_ht["CouponMoney"] = couponMoney;
                    result_ht["CouponPeriod"] = DateTime.Now.Date.AddDays(CouponPeriod).ToString("yyyy-MM-dd");
                    //响应数据到客户端
                    context.Response.Write(SiteHelper.GetJsonFromHashTable2(result_ht, "success", "获取数据成功", "GetInviteCouponInfo"));
                    break;
                case "SendVerificationCode":
                    string mobie = context.Request["Mobile"].ToString().Trim();
                    string code = "";
                    if (context.Request["Code"] != null)
                    {
                        code = context.Request["Code"].ToString().Trim();
                    }
                    else
                    {
                        context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "faild", "参数错误", "SendVerificationCode"));
                        context.Response.End();
                        break;
                    }
                    string session_code = context.Session["dt_session_code"].ToString();
                    if (code.ToLower() != session_code.ToLower())
                    {

                        context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "faild", "图形验证码错误", "SendVerificationCode"));
                        context.Response.End();
                        break;
                    }
                    SmsInfoManager sim = new SmsInfoManager();
                    UserInfoManager uim = new UserInfoManager();
                    DataTable user = uim.GetUserInfoByBindPhone(mobie);
                    if (user != null && user.Rows.Count > 0)
                    {
                        context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "faild", "手机号已注册", "SendVerificationCode"));
                        context.Response.End();
                        break;
                    }
                    bool sendResult = sim.SendVerificationCode(mobie);
                    if (sendResult)
                    {
                        context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "success", "发送验证码成功", "SendVerificationCode"));
                        context.Response.End();
                        break;
                    }
                    else
                    {
                        context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "faild", "发送验证码失败", "SendVerificationCode"));
                        context.Response.End();
                        break;
                    }
                case "NewUserRegister":
                    sim = new SmsInfoManager();
                    UserLoginLogManager ullm = new UserLoginLogManager();
                    uim = new UserInfoManager();
                    settingManager = new SysSettingManager();
                    string mobile = context.Request["Mobile"].ToString().Trim();
                    string verificationCode = context.Request["VerificationCode"].ToString().Trim();
                    string invite_userid = context.Request["InviteUserID"].ToString().Trim();

                    string vcode = sim.GetVerCodeByMobile(mobile);
                    if (verificationCode != vcode)
                    {
                        context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "faild", "手机验证码错误", "NewUserRegister"));
                        context.Response.End();
                        return;
                    }

                    user = uim.GetUserInfoByBindPhone(mobile);
                    if (user != null && user.Rows.Count > 0)
                    {
                        context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "faild", "用户已注册", "NewUserRegister"));
                        context.Response.End();
                        return;
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
                        newuser["InviteUserID"] = invite_userid;
                        int num = uim.AddUserInfo(newuser);
                        if (num > -1)
                        {
                            user = uim.GetUserInfoByBindPhone(mobile);

                            //新注册用户赠送优惠券
                            couponNum = 0;
                            int.TryParse(settingManager.GetValueByKey("RegCouponNum"), out couponNum);
                            couponMoney = 0.00m;
                            decimal.TryParse(settingManager.GetValueByKey("RegCouponMoney"), out couponMoney);
                            UserCouponManager couponManager = new UserCouponManager();
                            if (couponNum > 0 && couponMoney > 0)
                            {
                                couponManager.NewUserGiveCoupons(user.Rows[0]["ID"].ToString());
                            }

                            context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "success", "新用户注册成功", "NewUserRegister"));
                            context.Response.End();
                            return;
                        }
                        else
                        {
                            context.Response.Write(SiteHelper.GetJsonFromHashTable2(null, "faild", "手机号错误", "NewUserRegister"));
                            context.Response.End();
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
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