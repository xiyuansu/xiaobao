using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using cn.jpush.api;
using cn.jpush.api.push;
using cn.jpush.api.push.mode;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using YR.Common.DotNetCode;
using YR.Common.DotNetConfig;
using YR.Common.DotNetLog;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;
using YR.Web.App_Code;

namespace YR.Web.jobs
{
    /// <summary>
    /// 支付订单检测，定时检测用户支付订单状态，进行补单操作
    /// </summary>
    public class OrderPayQuery : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(OrderPayQuery));

        /// <summary>
        /// 支付订单检测
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime begintime = DateTime.Now.Date;
                DateTime endtime = begintime.AddHours(24);

                List<string> saasList=ConfigHelper.GetSaasList();
                foreach(string saas in saasList)
                {
                    try
                    {
                        MyWorkerRequest.CreateHttpContext(saas, "", "");

                        UserFinancialManager ufm = new UserFinancialManager();
                        UserInfoManager uim = new UserInfoManager();
                        DataTable dt = ufm.GetUserFinancialList(null,begintime,endtime, Asiasofti.SmartVehicle.Common.Enum.UserFinancialState.NewSubmit,null);
                        foreach (DataRow dr in dt.Rows)
                        {
                            string financial_id = dr["ID"].ToString();
                            string user_id = dr["UserID"].ToString();
                            string order_num = dr["OrderNum"].ToString();
                            decimal changes_amount=0.00m;
                            decimal.TryParse(dr["ChangesAmount"].ToString(),out changes_amount);
                            changes_amount = Math.Abs(changes_amount);
                            string order_payid = dr["OrderPayID"].ToString();
                            string operator_way = dr["OperatorWay"].ToString();
                            string changes_type = dr["ChangesType"].ToString();
                            if (changes_type == UserFinancialChangesType.Deposit.GetHashCode().ToString())
                            {
                                DepositPay(user_id, financial_id, changes_amount, order_payid, operator_way);
                            }
                            else if (changes_type == UserFinancialChangesType.Recharge.GetHashCode().ToString())
                            {
                                RechargePay(user_id, financial_id, changes_amount, order_payid, operator_way);
                            }
                            else if (changes_type == UserFinancialChangesType.Consumption.GetHashCode().ToString())
                            {
                                OrderPay(user_id, financial_id,order_num, changes_amount, order_payid, operator_way);
                            }
                        }
                    }
                    catch
                    {
                        Logger.Info("支付订单检测任务(OrderPayQuery:" + saas + ")失败");
                    }
                }
            }
            catch
            {
                Logger.Info("支付订单检测任务(OrderPayQuery)失败");
            }
        }

        private bool RechargePay(string user_id, string financial_id, decimal changes_amount, string order_payid, string operator_way)
        {
            bool isSuccess = false;
            try
            {
                UserInfoManager uim = new UserInfoManager();
                if (operator_way == UserFinancialOperatorWay.WeixinPay.GetHashCode().ToString())
                {
                    WxOrderQuery orderQuery = new WxOrderQuery();
                    OrderQueryResult queryResult = orderQuery.Query(order_payid);
                    if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                    {
                        Hashtable hashuf = new Hashtable();
                        hashuf["ID"] = financial_id;
                        hashuf["UserID"] = user_id;
                        hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                        hashuf["TradeNo"] = queryResult.transaction_id;
                        hashuf["TotalFee"] = queryResult.total_fee;
                        hashuf["PayWay"] = UserFinancialOperatorWay.WeixinPay;
                        decimal changesAmount = changes_amount;
                        if (Math.Abs(changesAmount) == queryResult.total_fee)
                            isSuccess = uim.RechargeCallBack(hashuf);
                    }
                }
                else if (operator_way == UserFinancialOperatorWay.Alipay.GetHashCode().ToString())
                {
                    AlipayOrderQuery orderQuery = new AlipayOrderQuery();
                    OrderQueryResult queryResult = orderQuery.Query(order_payid);
                    if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                    {
                        Hashtable hashuf = new Hashtable();
                        hashuf["ID"] = financial_id;
                        hashuf["UserID"] = user_id;
                        hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                        hashuf["TradeNo"] = queryResult.transaction_id;
                        hashuf["TotalFee"] = queryResult.total_fee;
                        hashuf["PayWay"] = UserFinancialOperatorWay.Alipay;
                        decimal changesAmount = changes_amount;
                        if (Math.Abs(changesAmount) == queryResult.total_fee)
                            isSuccess = uim.RechargeCallBack(hashuf);
                    }
                }
                return isSuccess;
            }
            catch
            {
                return false;
            }
        }

        private bool DepositPay(string user_id, string financial_id, decimal changes_amount, string order_payid, string operator_way)
        {
            bool isSuccess = false;
            try
            {
                UserInfoManager uim = new UserInfoManager();
                if (operator_way == UserFinancialOperatorWay.WeixinPay.GetHashCode().ToString())
                {
                    WxOrderQuery orderQuery = new WxOrderQuery();
                    OrderQueryResult queryResult = orderQuery.Query(order_payid);
                    if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                    {
                        Hashtable hashuf = new Hashtable();
                        hashuf["ID"] = financial_id;
                        hashuf["UserID"] = user_id;
                        hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                        hashuf["TradeNo"] = queryResult.transaction_id;
                        hashuf["TotalFee"] = queryResult.total_fee;
                        hashuf["PayWay"] = UserFinancialOperatorWay.WeixinPay;
                        decimal changesAmount = changes_amount;
                        if (Math.Abs(changesAmount) == queryResult.total_fee)
                            isSuccess = uim.DepositCallBack(hashuf);
                    }
                }
                else if (operator_way == UserFinancialOperatorWay.Alipay.GetHashCode().ToString())
                {
                    AlipayOrderQuery orderQuery = new AlipayOrderQuery();
                    OrderQueryResult queryResult = orderQuery.Query(order_payid);
                    if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                    {
                        Hashtable hashuf = new Hashtable();
                        hashuf["ID"] = financial_id;
                        hashuf["UserID"] = user_id;
                        hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                        hashuf["TradeNo"] = queryResult.transaction_id;
                        hashuf["TotalFee"] = queryResult.total_fee;
                        hashuf["PayWay"] = UserFinancialOperatorWay.Alipay;
                        decimal changesAmount = changes_amount;
                        if (Math.Abs(changesAmount) == queryResult.total_fee)
                            isSuccess = uim.DepositCallBack(hashuf);
                    }
                }
                return isSuccess;
            }
            catch
            {
                return false;
            }
        }

        private bool OrderPay(string user_id, string financial_id,string order_num, decimal changes_amount, string order_payid, string operator_way)
        {
            bool isSuccess = false;
            try
            {
                UserInfoManager uim = new UserInfoManager();
                if (operator_way == UserFinancialOperatorWay.WeixinPay.GetHashCode().ToString())
                {
                    WxOrderQuery orderQuery = new WxOrderQuery();
                    OrderQueryResult queryResult = orderQuery.Query(order_payid);
                    if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                    {
                        Hashtable hashuf = new Hashtable();
                        hashuf["ID"] = financial_id;
                        hashuf["OrderNum"] = order_num;
                        hashuf["UserID"] = user_id;
                        hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                        hashuf["TradeNo"] = queryResult.transaction_id;
                        hashuf["TotalFee"] = queryResult.total_fee;
                        hashuf["PayWay"] = UserFinancialOperatorWay.WeixinPay;
                        decimal changesAmount = changes_amount;
                        if (Math.Abs(changesAmount) == queryResult.total_fee)
                            isSuccess = uim.OrderPayCallback(hashuf);
                    }
                }
                else if (operator_way == UserFinancialOperatorWay.Alipay.GetHashCode().ToString())
                {
                    AlipayOrderQuery orderQuery = new AlipayOrderQuery();
                    OrderQueryResult queryResult = orderQuery.Query(order_payid);
                    if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                    {
                        Hashtable hashuf = new Hashtable();
                        hashuf["ID"] = financial_id;
                        hashuf["OrderNum"] = order_num;
                        hashuf["UserID"] = user_id;
                        hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                        hashuf["TradeNo"] = queryResult.transaction_id;
                        hashuf["TotalFee"] = queryResult.total_fee;
                        hashuf["PayWay"] = UserFinancialOperatorWay.Alipay;
                        decimal changesAmount = changes_amount;
                        if (Math.Abs(changesAmount) == queryResult.total_fee)
                            isSuccess = uim.OrderPayCallback(hashuf);
                    }
                }
                return isSuccess;
            }
            catch
            {
                return false;
            }
        }

    }
}