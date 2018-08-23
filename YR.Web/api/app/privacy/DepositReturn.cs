using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 申请退还押金
    /// </summary>
    public class DepositReturn : IApiAction2
    {
        //用户id
        private string uid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();

                SysSettingManager settingManager = new SysSettingManager();
                UserInfoManager uim = new UserInfoManager();
                Hashtable user = uim.GetUserInfoByUserID(uid);
                if (user == null || user.Keys.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您不是有效会员");
                }

                OrdersManager om = new OrdersManager();
                Hashtable unpaid_ht = om.GetUnPaidOrderByUser(uid);
                if (unpaid_ht != null && unpaid_ht.Keys.Count > 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "存在未支付订单,申请押金退款失败");
                }
                Hashtable uservehicle_ht = om.GetUserCurrentOrderVehicle(uid);
                if (uservehicle_ht != null && uservehicle_ht.Keys.Count > 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "存在进行中的订单,申请押金退款失败");
                }

                DepositReturnManager depositManager = new DepositReturnManager();
                bool result = depositManager.ApplyReturnDeposit(uid);
                if (result)
                {
                    string returnDepositMode = settingManager.GetValueByKey("ReturnDepositMode");
                    //押金退还模式，1即时退，0后台退
                    if (returnDepositMode == "1")
                    {
                        Hashtable apply_ht = depositManager.GetAppyInfoByUid(uid);
                        Refund(apply_ht["ID"].ToString());
                    }
                    return SiteHelper.GetJsonFromHashTable(null, "success", "成功申请押金退款", "data");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "申请押金退款失败");
                }
            }
        }

        /// <summary>
        /// 秒退款操作
        /// </summary>
        /// <param name="applyid"></param>
        /// <returns></returns>
        private bool Refund(string applyid)
        {
            try
            {
                bool result = false;
                DepositReturnManager depositManager = new DepositReturnManager();
                Hashtable ht = depositManager.GetDataByID(applyid);
                string state = SiteHelper.GetHashTableValueByKey(ht, "State");
                string depositMoney = SiteHelper.GetHashTableValueByKey(ht, "DepositMoney");
                string depositPayWay = SiteHelper.GetHashTableValueByKey(ht, "DepositPayWay");
                string depositTradeNo = SiteHelper.GetHashTableValueByKey(ht, "DepositTradeNo");
                decimal refundMoney = 0.00m;
                refundMoney = decimal.Parse(depositMoney);
                string errmsg = "";
                string code = "";
                if (depositPayWay == "1")
                {
                    //查询支付结果 支付宝退款
                    AlipayRefund alipay = new AlipayRefund();
                    result = alipay.Refund2(depositTradeNo, refundMoney, out errmsg, out code);
                }
                else if (depositPayWay == "2")
                {
                    //微信退款
                    WxRefund wxpay = new WxRefund();
                    result = wxpay.refund2(depositTradeNo, refundMoney, out errmsg);
                }
                else if (depositPayWay == "5")
                {
                    //微信公众号退款
                    api.weapp.pay.wxpay.WxRefund wxpay = new api.weapp.pay.wxpay.WxRefund();
                    result = wxpay.refund2(depositTradeNo, refundMoney, out errmsg);
                }
                if (result)
                {
                    result = depositManager.HandleReturnDeposit(applyid, "2", "", "系统");
                }
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}