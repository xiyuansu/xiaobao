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
using YR.Web.api.weapp.pay.wxpay;

namespace YR.Web.api.weapp.privacy
{
    /// <summary>
    /// 充值支付
    /// </summary>
    public class RechargePay : IApiAction2
    {
        //用户id
        private string uid = string.Empty;

        //支付方式,1:支付宝,2:微信支付
        private int payway = 0;

        //交易金额
        private decimal money = 0.00m;

        private Hashtable res = null;

        private string openID;

        private string ip;

        private string weixinNotifyUrl;

        private string alipayNotifyUrl;

        private string webRoot;

        private static Log Logger = LogFactory.GetLogger(typeof(RechargePay));

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["PayWay"] == null || res["PayWay"].ToString().Trim().Length <= 0 ||
                res["Money"] == null || res["Money"].ToString().Trim().Length <= 0 ||
                res["OpenID"] == null || res["OpenID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                openID = res["OpenID"].ToString().Trim();
                ip = HttpContext.Current.Request.UserHostAddress;
                int.TryParse(res["PayWay"].ToString().Trim(), out payway);
                decimal.TryParse(res["Money"].ToString().Trim(), out money);

                //在充值时进行身份验证
                UserInfoManager uim = new UserInfoManager();
                Hashtable user = uim.GetUserInfoByUserID(uid);
                if (user == null || user.Keys.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您不是有效会员");
                }
                else
                {
                    //验证用户是否经过身份认证
                    if ("4".CompareTo(user["REALNAMECERTIFICATION"].ToString()) != 0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前未完成实名认证，请完成实名认证后重试");
                    }
                    //验证用户状态是否有效
                    if ("0".CompareTo(user["USERSTATE"].ToString()) == 0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前已被禁用无法充值");
                    }
                }
                //----------------------------------------------------------------------------------

                webRoot = SiteHelper.GetWebRoot();
                weixinNotifyUrl = string.Format("{0}{1}", webRoot, "/api/weapp/pay/wxpay/recharge_notify_url.aspx");
                alipayNotifyUrl = string.Format("{0}{1}", webRoot, "/api/weapp/pay/alipay/recharge_notify_url.aspx");

                string result = "";
                switch (payway)
                {
                    case 1:
                        result = AliPay(uid, money);
                        break;
                    case 2:
                        result = WxPay(uid, money);
                        break;
                    default:
                        break;
                }
                return result;
            }
        }

        //支付宝
        private string AliPay(string uid, decimal money)
        {
            UserFinancialManager ufm = new UserFinancialManager();
            Hashtable hashuf = new Hashtable();
            hashuf["ID"] = CommonHelper.GetGuid;
            hashuf["UserID"] = uid;
            hashuf["OrderNum"] = SiteHelper.GenerateOrderNum();
            hashuf["OrderPayID"] = SiteHelper.GeneratePayID();
            hashuf["ChangesAmount"] = money;
            hashuf["ChangesTime"] = SiteHelper.GetWebServerCurrentTime();
            hashuf["ChangesType"] = (int)UserFinancialChangesType.Recharge;
            hashuf["Remark"] = "用户充值";
            hashuf["Operator"] = uid;
            hashuf["OperatorType"] = (int)UserFinancialOperatorType.User;
            hashuf["OperatorWay"] = (int)UserFinancialOperatorWay.Alipay;
            hashuf["State"] = (int)UserFinancialState.NewSubmit;
            bool isSuccess = ufm.AddOrEditUserFinancialInfo(hashuf, null);
            if (isSuccess)
            {
                Hashtable uf = ufm.GetUserFinancialPayInfoByPayID(hashuf["OrderPayID"].ToString());
                if (uf == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
                }
                else
                {
                    Hashtable result = new Hashtable();
                    result["PayWay"] = payway;
                    result["orderInfo"] = YR.Web.api.app.pay.alipay.Core.GetOrderInfo(hashuf["OrderPayID"].ToString(), "余额充值", string.Format("用户充值{0:N2}元", money), alipayNotifyUrl, money);
                    Logger.Info("RechargePay,AliPay orderInfo:" + result["orderInfo"]);
                    return SiteHelper.GetJsonFromHashTable(result, "success", "生成支付订单成功", "RechargePay");
                }
            }
            else
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
            }
        }

        //微信
        private string WxPay(string uid, decimal money)
        {
            UserFinancialManager ufm = new UserFinancialManager();
            Hashtable hashuf = new Hashtable();
            hashuf["ID"] = CommonHelper.GetGuid;
            hashuf["UserID"] = uid;
            hashuf["OrderNum"] = SiteHelper.GenerateOrderNum();
            hashuf["OrderPayID"] = SiteHelper.GeneratePayID();
            hashuf["ChangesAmount"] = money;
            hashuf["ChangesTime"] = SiteHelper.GetWebServerCurrentTime();
            hashuf["ChangesType"] = (int)UserFinancialChangesType.Recharge;
            hashuf["Remark"] = "用户充值";
            hashuf["Operator"] = uid;
            hashuf["OperatorType"] = (int)UserFinancialOperatorType.User;
            hashuf["OperatorWay"] = (int)UserFinancialOperatorWay.WeixinPubPay;
            hashuf["State"] = (int)UserFinancialState.NewSubmit;
            bool isSuccess = ufm.AddOrEditUserFinancialInfo(hashuf, null);
            if (isSuccess)
            {
                Hashtable uf = ufm.GetUserFinancialPayInfoByPayID(hashuf["OrderPayID"].ToString());
                if (uf == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
                }
                else
                {
                    WxUtil wxUtil = new WxUtil();
                    string prepayid = "", sign = "";
                    prepayid = wxUtil.GetPrepayId(hashuf["OrderPayID"].ToString(), "余额充值", money, weixinNotifyUrl,ip,openID);
                    Hashtable result = new Hashtable();
                    result["PayWay"] = payway;
                    result["appId"] = WxConfig.AppId;
                    result["timeStamp"] = wxUtil.genTimeStamp();
                    result["nonceStr"] = wxUtil.genNonceStr();
                    result["package"] = "prepay_id=" + prepayid;
                    result["signType"] = "MD5";

                    SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                    dic.Add("appId", result["appId"].ToString());
                    dic.Add("timeStamp", result["timeStamp"].ToString());
                    dic.Add("nonceStr", result["nonceStr"].ToString());
                    dic.Add("package", result["package"].ToString());
                    dic.Add("signType", result["signType"].ToString());
                    sign = wxUtil.getSign(dic);

                    result["sign"] = sign;
                    Logger.Info("RechargePay,WxPay prepayId:" + result["prepayId"]);
                    return SiteHelper.GetJsonFromHashTable(result, "success", "生成支付订单成功", "RechargePay");
                }
            }
            else
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
            }
        }
    }
}