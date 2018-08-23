using System;
using System.Collections;
using YR.Common.DotNetBean;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetUI;
using Asiasofti.SmartVehicle.Common;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;
using YR.Common.DotNetLog;

namespace YR.Web.Manage.UserManage
{
    public partial class UserDepositReturn_Form : System.Web.UI.Page
    {
        private string _key;

        private DepositReturnManager depositManager = new DepositReturnManager();
        private static Log Logger = LogFactory.GetLogger(typeof(UserDepositReturn_Form));

        protected void Page_Load(object sender, EventArgs e)
        {
            _key = Request["key"];
            if (!IsPostBack)
            {
                InitData();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            Hashtable ht = depositManager.GetDataByID(_key);
            if (ht.Count > 0 && ht != null)
            {
                ControlBindHelper.SetWebControls(this.Page, ht);
                if (ht["STATE"].ToString().Equals("2") || ht["STATE"].ToString().Equals("3"))
                {
                    State.Disabled = true;
                    Remark.Enabled = false;
                    Save.Visible = false;
                    Save.Enabled = false;
                }
                DepositPayWay.Text = Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserFinancialOperatorWay), Convert.ToInt32(ht["DEPOSITPAYWAY"].ToString()));
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = false;
                int returnType = 0;
                Hashtable ht = depositManager.GetDataByID(_key);
                string depositMoney = SiteHelper.GetHashTableValueByKey(ht, "DepositMoney");
                string depositPayWay = SiteHelper.GetHashTableValueByKey(ht, "DepositPayWay");
                string depositTradeNo = SiteHelper.GetHashTableValueByKey(ht, "DepositTradeNo");
                string realName = SiteHelper.GetHashTableValueByKey(ht, "RealName");
                string bindPhone = SiteHelper.GetHashTableValueByKey(ht, "BindPhone");
                decimal refundMoney = 0.00m;
                refundMoney = decimal.Parse(depositMoney);
                if (State.Value == "2")
                {
                    string errmsg = "";
                    string code = "";
                    if (depositPayWay == "1")
                    {
                        //支付宝退款
                        AlipayRefund alipay = new AlipayRefund();
                        result = alipay.Refund2(depositTradeNo, refundMoney, out errmsg, out code);
                        Logger.Warn(string.Format("{0},{1}支付宝退款:{2}", realName, bindPhone, errmsg));
                        if (!result && "40004".Equals(code))
                        {
                            //ShowMsgHelper.Alert_Error("支付宝退款失败:" + errmsg + "！");
                            //return;

                            //判断是否保存了buyer_id
                            string buyer_id = SiteHelper.GetHashTableValueByKey(ht, "buyer_id");
                            if (string.IsNullOrEmpty(buyer_id))
                            {
                                //buyer_id为空需要查询订单
                                AlipayOrderQuery orderQuery = new AlipayOrderQuery();
                                OrderQueryResult queryResult = orderQuery.QueryByTradeNO(depositTradeNo);
                                buyer_id = queryResult.buyer_user_id;
                            }

                            if (!string.IsNullOrEmpty(buyer_id))
                            {
                                //执行单笔转账到支付宝账户
                                result = alipay.Transfer(depositTradeNo, buyer_id, refundMoney, out errmsg);
                                Logger.Warn(string.Format("{0},{1}支付宝转账退款:{2}", realName, bindPhone, errmsg));
                                returnType = 1;
                                if (!result)
                                {
                                    ShowMsgHelper.Alert_Error("支付宝转账退款失败:" + errmsg + "！");
                                    return;
                                }
                            }
                        }
                    }
                    else if (depositPayWay == "2")
                    {
                        //微信退款
                        WxRefund wxpay = new WxRefund();
                        result = wxpay.refund2(depositTradeNo, refundMoney, out errmsg);
                        if (!result)
                        {
                            ShowMsgHelper.Alert_Error("微信退款失败:" + errmsg + "！");
                            return;
                        }
                    }
                    else if (depositPayWay == "5")
                    {
                        //微信公众号退款
                        api.weapp.pay.wxpay.WxRefund wxpay = new api.weapp.pay.wxpay.WxRefund();
                        result = wxpay.refund2(depositTradeNo, refundMoney, out errmsg);
                        if (!result)
                        {
                            ShowMsgHelper.Alert_Error("微信公众号退款失败:" + errmsg + "！");
                            return;
                        }
                    }
                }
                else
                {
                    result = true;
                }
                if (result)
                {
                    result = depositManager.HandleReturnDeposit(_key, State.Value, Remark.Text.Trim(), RequestSession.GetSessionUser().UserName.ToString(), returnType);
                    if (result)
                    {
                        ShowMsgHelper.AlertMsgNoFresh("操作成功！");
                    }
                    else
                    {
                        ShowMsgHelper.Alert_Error("操作失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void Transfer_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = true;
                int returnType = 1;
                /*Hashtable ht = depositManager.GetDataByID(_key);
                string depositMoney = SiteHelper.GetHashTableValueByKey(ht, "DepositMoney");
                string depositPayWay = SiteHelper.GetHashTableValueByKey(ht, "DepositPayWay");
                string depositTradeNo = SiteHelper.GetHashTableValueByKey(ht, "DepositTradeNo");
                decimal refundMoney = 0.00m;
                refundMoney = decimal.Parse(depositMoney);*/
                if (result)
                {
                    //result = depositManager.HandleReturnDeposit(_key, State.Value, Remark.Text.Trim(), RequestSession.GetSessionUser().UserName.ToString(), returnType);
                    result = depositManager.HandleReturnedDeposit(_key, RequestSession.GetSessionUser().UserName.ToString());
                    if (result)
                    {
                        ShowMsgHelper.AlertMsgNoFresh("操作成功！");
                    }
                    else
                    {
                        ShowMsgHelper.Alert_Error("操作失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}