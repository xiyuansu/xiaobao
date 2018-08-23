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
    /// 指令控制（启动 设防 鸣笛）
    /// </summary>
    public class OperateCarByBaoJia : IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();
            string plate_no = params_ht["plate_no"].ToString();
            string order_no = params_ht["order_no"].ToString();
            int op_type = int.Parse(params_ht["op_type"].ToString());

            bool result=false;
            OrdersManager om = new OrdersManager();
            BaoJiaManager mgr = new BaoJiaManager();
            Hashtable order_ht = mgr.GetOrderInfo(order_no);
            if (order_ht != null && order_ht.Keys.Count > 0)
            {
                string order_state = SiteHelper.GetHashTableValueByKey(order_ht, "OrderState");
                string vid = SiteHelper.GetHashTableValueByKey(order_ht, "VehicleID");
                VehicleManager vm = new VehicleManager();
                if (order_state == "1")
                {
                    switch (op_type)
                    {
                        case 1:
                            result=vm.OpenVehicle(vid);
                            //如果是首次开车订单StartTime设为当前时间
                            if (result && string.IsNullOrEmpty(order_ht["STARTTIME"].ToString()))
                            {
                                mgr.SetCurrentOrderStartTime(order_ht["ID"].ToString());
                                om.UpdateOrderGPS(order_ht["ID"].ToString(), true);
                            }
                            break;
                        case 2:
                            result = vm.CloseVehicle(vid);
                            break;
                        case 3:
                            result = vm.FindVehicle(vid);
                            break;
                        default:
                            break;
                    }
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