using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using YR.Common.DotNetConfig;
using YR.Common.DotNetLog;
using YR.Web.App_Code;

namespace YR.Web.jobs
{
    /// <summary>
    /// 第三方订单预约超时自动取消任务
    /// </summary>
    public class CancelOrders : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(CancelOrders));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                int minutes = 10;
                int.TryParse(context.JobDetail.JobDataMap.Get("minutes").ToString(), out minutes);
                foreach (string saas in ConfigHelper.GetSaasList())
                {
                    try
                    {
                        MyWorkerRequest.CreateHttpContext(saas, "", "");
                        OrdersManager manager = new OrdersManager();
                        DataTable dt = manager.GetAwaitCancelOrder(minutes);
                        foreach (DataRow row in dt.Rows)
                        {
                            string ordernum = row["OrderNum"].ToString();
                            if (CancelOrder(ordernum,1))
                                Logger.Info("超过"+ minutes+"分钟自动取消订单成功, 订单号:" + ordernum+",订单创建时间:"+ row["CreateTime"].ToString());
                            else
                                Logger.Info("超过"+ minutes+"分钟自动取消订单失败");
                        }
                        continue;
                    }
                    catch
                    {
                        Logger.Info("超过" + minutes + "分钟自动取消订单(CancelOrders:" + saas + ")失败");
                        continue;
                    }
                }
            }
            catch
            {
                Logger.Info("超时自动取消订单(CancelOrders)失败");
            }
        }

        /// <summary>
        /// 根据订单号取消订单
        /// </summary>
        /// <param name="order_no">订单号</param>
        /// <param name="type">1 系统自动取消订单 0 客户取消订单 </param>
        /// <returns></returns>
        private bool CancelOrder(string ordernum,int type)
        {
            OrdersManager ordersManager = new OrdersManager();
            return ordersManager.CancelOrder(ordernum,type);
        }

        /*
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                int minutes = 10;
                int.TryParse(context.JobDetail.JobDataMap.Get("minutes").ToString(), out minutes);
                foreach (string saas in ConfigHelper.GetSaasList())
                {
                    try
                    {
                        MyWorkerRequest.CreateHttpContext(saas, "", "");
                        BaoJiaManager manager = new BaoJiaManager();
                        DataTable dt = manager.GetAwaitCancelOrder(minutes);
                        foreach (DataRow row in dt.Rows)
                        {
                            string out_order_no = row["OutOrderNo"].ToString();
                            string ordernum = row["OrderNum"].ToString();
                            if (CancelOrder(out_order_no))
                                Logger.Info("取消第三方订单成功,订单号:" + ordernum);
                            else
                                Logger.Info("取消第三方订单失败");
                        }
                        continue;
                    }
                    catch
                    {
                        Logger.Info("取消第三方订单(CancelOrders:" + saas + ")失败");
                        continue;
                    }
                }
            }
            catch
            {
                Logger.Info("取消第三方订单(CancelOrders)失败");
            }
        }

        /// <summary>
        /// 根据订单号取消订单
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        private bool CancelOrder(string out_order_no)
        {
            bool result = false;
            VehicleManager vehicleManager = new VehicleManager();
            BaoJiaManager baojiaManager = new BaoJiaManager();
            Hashtable orderInfo = baojiaManager.GetOrderInfo(out_order_no);
            if (orderInfo != null && orderInfo.Keys.Count > 0 && SiteHelper.GetHashTableValueByKey(orderInfo, "OrderState") == "1")
            {
                string orderid = SiteHelper.GetHashTableValueByKey(orderInfo, "ID");
                string vid = SiteHelper.GetHashTableValueByKey(orderInfo, "VehicleID");
                vehicleManager.CloseVehicle(vid);
                result = baojiaManager.CancelOrder(orderid);
            }
            return result;
        }
        */

    }

}