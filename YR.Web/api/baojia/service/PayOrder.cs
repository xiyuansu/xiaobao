using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace YR.Web.api.baojia.service
{
    /// <summary>
    /// 支付通知
    /// </summary>
    public class PayOrder : IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();
            string order_no = params_ht["order_no"].ToString();
            string mobile = params_ht["mobile"].ToString();
            string money = params_ht["money"].ToString();
            long paytime = long.Parse(params_ht["paytime"].ToString());

            bool result = false;
            BaoJiaManager mgr = new BaoJiaManager();
            Hashtable order_ht=mgr.GetOrderInfo(order_no);
            if (order_ht != null && order_ht.Keys.Count > 0)
            {
                string pay_state = SiteHelper.GetHashTableValueByKey(order_ht, "PayState");
                if (pay_state == "0")
                {
                    Hashtable ht = new Hashtable();
                    ht["OrderNum"] = SiteHelper.GetHashTableValueByKey(order_ht, "OrderNum");
                    ht["TotalFee"] = money;
                    result = mgr.OrderPayCallback(ht);
                }
            }
            else
            {
                resp.code = "-1";
                resp.msg = "未找到相关订单";
            }

            if (result)
            {
                resp.code = "0";
                resp.msg = "成功";
            }
            else
            {
                resp.code = "-1";
                resp.msg = "失败";
            }

            return resp;
        }
    }

}