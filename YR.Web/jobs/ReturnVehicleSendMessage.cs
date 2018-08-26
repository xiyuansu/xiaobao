using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using YR.Common.DotNetCache;
using YR.Common.DotNetConfig;
using YR.Common.DotNetLog;
using YR.Web.App_Code;

namespace YR.Web.jobs
{
    /// <summary>
    /// 还车通知，超过指定时间后未还车发送还车通知
    /// </summary>
    public class ReturnVehicleSendMessage : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(ReturnVehicleSendMessage));


        /// <summary>
        /// 占用车辆超时还车通知
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                int minutes_timeout = 60;
                List<string> saasList = ConfigHelper.GetSaasList();
                int i = 0;
                foreach (string saas in saasList)
                {
                    if (i == 0)
                    {
                        ICache cache = null;
                        try
                        {
                            MyWorkerRequest.CreateHttpContext(saas, "", "");
                            OrdersManager om = new OrdersManager();
                            VehicleManager vm = new VehicleManager();
                            //DataTable dt = om.GetVehicleReturnOrderList(minutes_timeout);
                            DataTable dt = vm.GetInUseVehicles();
                            Logger.Debug(string.Format("开始车辆还车通知任务，车辆数：{0}，saas：{1}", dt.Rows.Count, saas));
                            UserInfoManager um = new UserInfoManager();
                            SmsInfoManager sim = new SmsInfoManager();
                            cache = CacheFactory.GetCache();
                            foreach (DataRow dr in dt.Rows)
                            {
                                string vehicleID = dr["ID"].ToString();
                                string lockState = dr["LockState"].ToString();
                                string vehicleName = dr["VehicleName"].ToString();
                                //string userID = dr["UserID"].ToString();
                                //string orderNum = dr["OrderNum"].ToString();
                                if (!string.IsNullOrEmpty(vehicleID))
                                {
                                    //Hashtable htVehicle = vm.GetVehicleInfoByID(vehicleID);
                                    Hashtable htOrder = om.GetInUseOrder(vehicleID);
                                    if (htOrder != null)
                                    {
                                        string userID = SiteHelper.GetHashTableValueByKey(htOrder, "UserID");
                                        //string lockState = SiteHelper.GetHashTableValueByKey(htVehicle, "LockState");
                                        //string vehicleName = SiteHelper.GetHashTableValueByKey(htVehicle, "VehicleName");
                                        string orderNum = SiteHelper.GetHashTableValueByKey(htOrder, "OrderNum");
                                        string createTimeStr = SiteHelper.GetHashTableValueByKey(htOrder, "CreateTime");
                                        DateTime createTime = DateTime.Parse(createTimeStr);
                                        TimeSpan span = DateTime.Now - createTime;
                                        if ("lock".Equals(lockState) && span.TotalMinutes > minutes_timeout)
                                        {
                                            Hashtable htUser = um.GetUserInfoByUserID(userID);
                                            if (htUser != null)
                                            {
                                                string mobile = SiteHelper.GetHashTableValueByKey(htUser, "BindPhone");
                                                string key = "Return_Vehicle_Message_" + mobile + "_" + vehicleID;
                                                string code_value = cache.Get<string>(key);
                                                if (string.IsNullOrEmpty(code_value))
                                                {
                                                    bool sendResult = sim.SendReturn(mobile);
                                                    if (sendResult)
                                                    {
                                                        Logger.Warn(string.Format("订单号{0}，车辆{1}还车提醒，发送给{2}成功。", orderNum, vehicleName, mobile));
                                                        DateTime dtDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                                                        cache.Set(key, 1, dtDay - DateTime.Now);
                                                    }
                                                    else
                                                    {
                                                        Logger.Error(string.Format("订单号{0}，车辆{1}还车提醒，发送给{2}失败。", orderNum, vehicleName, mobile));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (cache != null)
                            {
                                cache.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            if (cache != null)
                            {
                                cache.Dispose();
                            }
                            Logger.Error(string.Format("车辆还车通知任务(ReturnVehicleSendMessage：{0})失败，{1}，{2}", saas, e.Message, e.StackTrace));
                        }
                        i++;
                    }
                }
            }
            catch
            {
                Logger.Info("车辆还车通知任务(ReturnVehicleNotify)失败");
            }
        }
    }
}