using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections.Generic;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetData;
using Asiasofti.SmartVehicle.Common.Enum;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using Newtonsoft.Json;

namespace YR.Web.api.app.pay.alipay
{
    public partial class order_notify_url : System.Web.UI.Page
    {
        private static Log Logger = LogFactory.GetLogger(typeof(order_notify_url));

        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> sPara = GetRequestPost();
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                string notify_id = Request.Form["notify_id"];//获取notify_id
                string sign = Request.Form["sign"];//获取sign
                Logger.Warn(string.Format("订单支付,支付宝支付通知,结果{0}", JsonConvert.SerializeObject(sPara)));
                if (aliNotify.GetSignVeryfy(sPara, sign))
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码

                    //商户订单号
                    string out_trade_no = Request.Form["out_trade_no"];
                    //支付宝交易号
                    string trade_no = Request.Form["trade_no"];
                    //支付宝交易金额
                    string total_fee = Request.Form["total_amount"];
                    //支付宝用户号 支付宝账号对应的支付宝唯一用户号。以2088开头的纯16位数字
                    string buyer_id = Request.Form["buyer_id"];
                    //支付宝账号
                    string buyer_logon_id = Request.Form["buyer_logon_id"];

                    Logger.Warn(string.Format("订单支付,支付宝支付通知,订单号{0},支付单号{1},支付金额{2},支付宝用户号{3},支付宝账号{4}", out_trade_no, trade_no, total_fee, buyer_id, buyer_logon_id));

                    //交易状态
                    string trade_status = Request.Form["trade_status"];
                    if (trade_status == "TRADE_SUCCESS" || trade_status == "TRADE_FINISHED")
                    {
                        UserInfoManager uim = new UserInfoManager();
                        UserFinancialManager ufm = new UserFinancialManager();
                        DataTable userfinancial = ufm.GetUserFinancialInfoByPayId(out_trade_no);
                        if (userfinancial != null && userfinancial.Rows.Count == 1)
                        {
                            Hashtable hashufdb = DataTableHelper.DataRowToHashTable(userfinancial.Rows[0]);
                            Hashtable hashuf = new Hashtable();
                            hashuf["ID"] = SiteHelper.GetHashTableValueByKey(hashufdb, "ID");
                            hashuf["OrderNum"] = SiteHelper.GetHashTableValueByKey(hashufdb, "OrderNum");
                            hashuf["UserID"] = SiteHelper.GetHashTableValueByKey(hashufdb, "UserID");
                            hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                            hashuf["TradeNo"] = trade_no;
                            hashuf["TotalFee"] = decimal.Parse(total_fee);
                            hashuf["PayWay"] = UserFinancialOperatorWay.Alipay;
                            hashuf["buyer_id"] = buyer_id;
                            hashuf["buyer_logon_id"] = buyer_logon_id;
                            decimal changesAmount = 0.00m;
                            decimal.TryParse(SiteHelper.GetHashTableValueByKey(hashufdb, "ChangesAmount"), out changesAmount);
                            bool isSuccess = false;
                            if (Math.Abs(changesAmount) == decimal.Parse(total_fee))
                                isSuccess = uim.OrderPayCallback(hashuf);
                            if (isSuccess)
                            {
                                Response.Write("success");
                            }
                            else
                            {
                                Response.Write("fail");
                            }
                        }
                    }
                    else
                    {

                    }
                }
                else//验证失败
                {
                    Response.Write("fail");
                }
            }
            else
            {
                Response.Write("无通知参数");
            }
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArraytemp = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArraytemp.Add(requestItem[i], Request.Form[requestItem[i]]);
            }
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in sArraytemp)
            {
                sArray.Add(temp.Key, temp.Value);
            }
            return sArray;
        }
    }
}