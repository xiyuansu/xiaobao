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
    /// 订单支付
    /// </summary>
    public class OrderPay : IApiAction2
    {
        //用户id
        private string uid = string.Empty;

        //支付方式,1:支付宝,2:微信支付,3:余额支付
        private int payway = 0;

        //支付的订单号
        private string ordernum = string.Empty;

        private Hashtable res = null;

        private string weixinNotifyUrl;

        private string alipayNotifyUrl;

        private string webRoot;

        private static Log Logger = LogFactory.GetLogger(typeof(OrderPay));

        private string cacheKey = "OrderPay_";

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["PayWay"] == null || res["PayWay"].ToString().Trim().Length <= 0 ||
                res["OrderNum"] == null || res["OrderNum"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                int.TryParse(res["PayWay"].ToString().Trim(), out payway);
                cacheKey += uid;
                string ordernum = res["OrderNum"].ToString().Trim();
                string couponID = "";
                UserFinancialManager ufm = new UserFinancialManager();
                UserInfoManager uim = new UserInfoManager();
                OrdersManager om = new OrdersManager();
                UserCouponManager couponManager = new UserCouponManager();
                Hashtable order_ht = om.GetOrderInfoByNum(ordernum);
                if (order_ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到订单数据,支付失败");
                }
                int payState = int.Parse(SiteHelper.GetHashTableValueByKey(order_ht, "PayState"));
                int orderState = int.Parse(SiteHelper.GetHashTableValueByKey(order_ht, "OrderState"));
                decimal payMoney = decimal.Parse(SiteHelper.GetHashTableValueByKey(order_ht, "PayMoney"));
                decimal totalMoney = decimal.Parse(SiteHelper.GetHashTableValueByKey(order_ht, "TotalMoney"));

                if (OrderState.UnPay.GetHashCode() != orderState)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "订单支付失败");
                }
                if (totalMoney <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未产生支付费用,无需进行支付操作");
                }

                #region 检测前一笔未确认支付交易是否成功，成功则禁止重复支付

                Hashtable financial_ht = ufm.GetLatestUserFinancialInfo(uid,UserFinancialState.NewSubmit, UserFinancialChangesType.Consumption);
                if (financial_ht != null && financial_ht.Keys.Count > 0)
                {
                    string order_num = SiteHelper.GetHashTableValueByKey(financial_ht, "OrderNum");
                    string order_payid = SiteHelper.GetHashTableValueByKey(financial_ht, "OrderPayID");
                    string operator_way = SiteHelper.GetHashTableValueByKey(financial_ht, "OperatorWay");
                    if (operator_way == UserFinancialOperatorWay.WeixinPay.GetHashCode().ToString())
                    {
                        WxOrderQuery orderQuery = new WxOrderQuery();
                        OrderQueryResult queryResult = orderQuery.Query(order_payid);
                        if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                        {
                            Hashtable hashuf = new Hashtable();
                            hashuf["ID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "ID");
                            hashuf["OrderNum"] = SiteHelper.GetHashTableValueByKey(financial_ht, "OrderNum");
                            hashuf["UserID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "UserID");
                            hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                            hashuf["TradeNo"] = queryResult.transaction_id;
                            hashuf["TotalFee"] = queryResult.total_fee;
                            hashuf["PayWay"] = UserFinancialOperatorWay.WeixinPay;
                            decimal changesAmount = 0.00m;
                            decimal.TryParse(SiteHelper.GetHashTableValueByKey(financial_ht, "ChangesAmount"), out changesAmount);
                            bool isSuccess = false;
                            if (Math.Abs(changesAmount) == queryResult.total_fee)
                                isSuccess = uim.OrderPayCallback(hashuf);
                            if (isSuccess && ordernum==order_num)
                            {
                                return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前订单已成功支付");
                            }
                        }
                    }
                    else if (operator_way == UserFinancialOperatorWay.Alipay.GetHashCode().ToString())
                    {
                        AlipayOrderQuery orderQuery = new AlipayOrderQuery();
                        OrderQueryResult queryResult = orderQuery.Query(order_payid);
                        if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                        {
                            Hashtable hashuf = new Hashtable();
                            hashuf["ID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "ID");
                            hashuf["OrderNum"] = SiteHelper.GetHashTableValueByKey(financial_ht, "OrderNum");
                            hashuf["UserID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "UserID");
                            hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                            hashuf["TradeNo"] = queryResult.transaction_id;
                            hashuf["TotalFee"] = queryResult.total_fee;
                            hashuf["PayWay"] = UserFinancialOperatorWay.Alipay;
                            decimal changesAmount = 0.00m;
                            decimal.TryParse(SiteHelper.GetHashTableValueByKey(financial_ht, "ChangesAmount"), out changesAmount);
                            bool isSuccess = false;
                            if (Math.Abs(changesAmount) == queryResult.total_fee)
                                isSuccess = uim.OrderPayCallback(hashuf);
                            if (isSuccess && ordernum == order_num)
                            {
                                return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前订单已成功支付");
                            }
                        }
                    }
                }

                #endregion

                if (res["CouponID"] != null && res["CouponID"].ToString().Trim().Length > 0)
                {
                    couponID = res["CouponID"].ToString().Trim();
                    Hashtable coupon_ht= couponManager.GetDataByID(couponID);
                    if(coupon_ht==null || coupon_ht.Keys.Count==0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "使用骑行券失败,支付失败");
                    }
                    string orderid=SiteHelper.GetHashTableValueByKey(coupon_ht, "OrderID");
                    string begintime = SiteHelper.GetHashTableValueByKey(coupon_ht, "BeginTime");
                    string endtime = SiteHelper.GetHashTableValueByKey(coupon_ht, "EndTime");
                    string userid = SiteHelper.GetHashTableValueByKey(coupon_ht, "UserID");
                    decimal couponMoney=0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(coupon_ht, "Money"), out couponMoney);
                    if(userid!=uid)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "使用骑行券失败,支付失败");
                    }
                    if(!string.IsNullOrEmpty(orderid))
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "使用骑行券失败,支付失败");
                    }
                    if(DateTime.Parse(begintime)>DateTime.Now || DateTime.Parse(endtime)<DateTime.Now)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "使用骑行券失败,支付失败");
                    }
                    if (couponMoney>totalMoney)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "使用骑行券失败,支付失败");
                    }

                    Hashtable update_order_ht = new Hashtable();
                    update_order_ht["CouponID"] = couponID;
                    update_order_ht["CouponMoney"] = couponMoney;
                    update_order_ht["PayMoney"] = totalMoney - couponMoney;
                    bool rrr=om.AddOrEditOrdersInfo(update_order_ht, SiteHelper.GetHashTableValueByKey(order_ht, "ID"));
                }

                webRoot = SiteHelper.GetWebRoot();
                weixinNotifyUrl = string.Format("{0}{1}", webRoot, "/api/app/pay/wxpay/order_notify_url.aspx");
                alipayNotifyUrl = string.Format("{0}{1}", webRoot, "/api/app/pay/alipay/order_notify_url.aspx");

                string result = "";
                ICache cache = null;
                switch (payway)
                {
                    case 1:
                        cache = CacheFactory.GetCache();
                        if (!string.IsNullOrEmpty(cache.Get<string>(cacheKey)))
                        {
                            cache.Dispose();
                            Logger.Error("订单支付宝支付5秒内，" + uid);
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "5秒内请勿重复点击");
                        }
                        cache.Dispose();
                        result = AliPay(uid, ordernum);
                        break;
                    case 2:
                        cache = CacheFactory.GetCache();
                        if (!string.IsNullOrEmpty(cache.Get<string>(cacheKey)))
                        {
                            cache.Dispose();
                            Logger.Error("订单微信支付5秒内，" + uid);
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "5秒内请勿重复点击");
                        }
                        cache.Dispose();
                        result = WxPay(uid, ordernum);
                        break;
                    case 3:
                        result = BalancePay(uid, ordernum);
                        break;
                    default:
                        break;
                }
                return result;
            }
        }

        //支付宝
        private string AliPay(string uid, string ordernum)
        {
            OrdersManager om = new OrdersManager();
            Hashtable order_ht = om.GetOrderInfoByNum(ordernum);
            decimal money = 0.00m;
            decimal.TryParse(SiteHelper.GetHashTableValueByKey(order_ht, "PayMoney"), out money);

            UserFinancialManager ufm = new UserFinancialManager();
            Hashtable hashuf = new Hashtable();
            hashuf["ID"] = CommonHelper.GetGuid;
            hashuf["UserID"] = uid;
            hashuf["OrderNum"] = ordernum;
            hashuf["OrderPayID"] = SiteHelper.GeneratePayID();
            hashuf["ChangesAmount"] = -money;
            hashuf["ChangesTime"] = SiteHelper.GetWebServerCurrentTime();
            hashuf["ChangesType"] = (int)UserFinancialChangesType.Consumption;
            hashuf["Remark"] = "订单支付";
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
                    result["orderInfo"] = YR.Web.api.app.pay.alipay.Core.GetOrderInfo(hashuf["OrderPayID"].ToString(), "订单支付", string.Format("订单支付{0:N2}元", money), alipayNotifyUrl, money).Replace("\"", "\\\"");
                    Logger.Info("OrderPay,AliPay orderInfo:" + result["orderInfo"]);

                    ICache cache = CacheFactory.GetCache();
                    DateTime dt = DateTime.Now.AddSeconds(5);
                    cache.Set(cacheKey, uid, dt - DateTime.Now);
                    cache.Dispose();

                    return SiteHelper.GetJsonFromHashTable(result, "success", "生成支付订单成功", "OrderPay");
                }
            }
            else
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
            }
        }

        //微信
        private string WxPay(string uid, string ordernum)
        {
            OrdersManager om = new OrdersManager();
            Hashtable order_ht = om.GetOrderInfoByNum(ordernum);
            decimal money = 0.00m;
            decimal.TryParse(SiteHelper.GetHashTableValueByKey(order_ht, "PayMoney"), out money);

            UserFinancialManager ufm = new UserFinancialManager();
            Hashtable hashuf = new Hashtable();
            hashuf["ID"] = CommonHelper.GetGuid;
            hashuf["UserID"] = uid;
            hashuf["OrderNum"] = ordernum;
            hashuf["OrderPayID"] = SiteHelper.GeneratePayID();
            hashuf["ChangesAmount"] = -money;
            hashuf["ChangesTime"] = SiteHelper.GetWebServerCurrentTime();
            hashuf["ChangesType"] = (int)UserFinancialChangesType.Consumption;
            hashuf["Remark"] = "订单支付";
            hashuf["Operator"] = uid;
            hashuf["OperatorType"] = (int)UserFinancialOperatorType.User;
            hashuf["OperatorWay"] = (int)UserFinancialOperatorWay.WeixinPay;
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
                    prepayid = wxUtil.GetPrepayId(hashuf["OrderPayID"].ToString(), "订单支付", money, weixinNotifyUrl,HttpContext.Current.Request.UserHostAddress);
                    Hashtable result = new Hashtable();
                    result["PayWay"] = payway;
                    result["appId"] = pay.wxpay.WxConfig.AppId;
                    result["partnerId"] = pay.wxpay.WxConfig.MchId;
                    result["prepayId"] = prepayid;
                    result["packageValue"] = "Sign=WXPay";
                    result["nonceStr"] = wxUtil.genNonceStr();
                    result["timeStamp"] = wxUtil.genTimeStamp();

                    SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                    dic.Add("appid", result["appId"].ToString());
                    dic.Add("noncestr", result["nonceStr"].ToString());
                    dic.Add("package", result["packageValue"].ToString());
                    dic.Add("partnerid", result["partnerId"].ToString());
                    dic.Add("prepayid", result["prepayId"].ToString());
                    dic.Add("timestamp", result["timeStamp"].ToString());
                    sign = wxUtil.getSign(dic);

                    result["sign"] = sign;
                    Logger.Info("OrderPay,WxPay prepayId:" + result["prepayId"]);

                    ICache cache = CacheFactory.GetCache();
                    DateTime dt = DateTime.Now.AddSeconds(5);
                    cache.Set(cacheKey, uid, dt - DateTime.Now);
                    cache.Dispose();

                    return SiteHelper.GetJsonFromHashTable(result, "success", "生成支付订单成功", "OrderPay");
                }
            }
            else
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
            }
        }

        //余额
        private string BalancePay(string uid, string ordernum)
        {
            OrdersManager om = new OrdersManager();
            UserInfoManager uim = new UserInfoManager();

            Hashtable order_ht = om.GetOrderInfoByNum(ordernum);
            if (order_ht != null)
            {
                string strPay = string.Empty;
                if (uim.UserOrderPay(ordernum, out strPay))
                {
                    order_ht = om.GetOrderInfoByNum(ordernum);
                    Hashtable result_ht = new Hashtable();
                    result_ht["PayWay"] = payway;
                    result_ht["OrderNum"] = ordernum;
                    result_ht["PayMoney"] = SiteHelper.GetHashTableValueByKey(order_ht, "PayMoney");
                    return SiteHelper.GetJsonFromHashTable(result_ht, "success", strPay, "OrderPay");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
                }
            }
            else
            {
                return "";
            }
        }
    }
}