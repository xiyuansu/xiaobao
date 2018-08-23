using Asiasofti.SmartVehicle.Manager;
using Quartz;
using System;
using System.Data;
using YR.Common.DotNetLog;

namespace YR.Web.jobs
{
    /// <summary>
    /// 定时位移报警
    /// </summary>
    public class NoOrderMove : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(CancelOrders));

        public void Execute(IJobExecutionContext context)
        {
            string type = "无单位移";
            try
            {
                SmsInfoManager sim = new SmsInfoManager();
                VehicleManager vm = new VehicleManager();
                OPUserManager oum = new OPUserManager();
                DataTable dt = vm.GetNoOrderVehicles();
                DataTable users = oum.GetReceiveSMSUsers();
                foreach (DataRow row in dt.Rows)
                {
                    string vid = row["ID"].ToString();
                    string name = row["Name"].ToString();
                    string gpsNum = row["VehicleGPSNum"].ToString();
                    DateTime begin = DateTime.Now.AddMinutes(-10);
                    DateTime end = DateTime.Now;
                    DataTable dtList = vm.GetSpeedVehicleTrace(vid, begin, end);
                    if(dtList!=null&& dtList.Rows.Count >=10)
                    {
                        if (users != null && users.Rows.Count > 0)
                        {
                            foreach (DataRow user in users.Rows)
                            {
                                string mobile = user[0].ToString();
                                bool sendResult = sim.SendAlarm(mobile, gpsNum, name, type);
                                if (sendResult)
                                {
                                    Logger.Warn(string.Format("车辆{0}，{1}发生{2}报警，发送给{3}成功。", name, gpsNum, type, mobile));
                                }
                                else
                                {
                                    Logger.Warn(string.Format("车辆{0}，{1}发生{2}报警，发送给{3}失败。", name, gpsNum, type, mobile));
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Error("无单位移报警发送短信失败，"+e.Message);
            }
        }
    }
}