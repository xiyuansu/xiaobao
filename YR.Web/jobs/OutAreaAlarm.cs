using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using Quartz;
using System;
using System.Collections;
using System.Data;
using YR.Common.DotNetCache;
using YR.Common.DotNetLog;
using YR.Common.DotNetConfig;
using YR.Web.App_Code;

namespace YR.Web.jobs
{
    /// <summary>
    /// 越界报警短信通知
    /// </summary>
    public class OutAreaAlarm : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(OverSpeedAlarm));

        /// <summary>
        /// 越界报警短信通知
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            ICache cache = null;
            string type = "越界";
            try
            {
                foreach (string saas in ConfigHelper.GetSaasList())
                {
                    try
                    {
                        MyWorkerRequest.CreateHttpContext(saas, "", "");

                        SmsInfoManager sim = new SmsInfoManager();
                        OPUserManager oum = new OPUserManager();
                        DataTable users = oum.GetReceiveSMSUsers();
                        VehicleAlarmManager vam = new VehicleAlarmManager();
                        VehicleManager vm = new VehicleManager();
                        DataTable dataTable = vam.GetOutAreaList();
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            cache = CacheFactory.GetCache();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                string vehicleID = row[0].ToString();
                                Hashtable vehicle = vm.GetVehicleInfoByID(vehicleID);
                                string gpsNum = SiteHelper.GetHashTableValueByKey(vehicle, "VehicleGPSNum").ToUpper();
                                string name = SiteHelper.GetHashTableValueByKey(vehicle, "Name").ToUpper();
                                string alarmCount = row[1].ToString();
                                if (!string.IsNullOrEmpty(alarmCount))
                                {
                                    int count = 0;
                                    int.TryParse(alarmCount, out count);
                                    if (count >=4)
                                    {
                                        if (users != null && users.Rows.Count > 0)
                                        {
                                            foreach (DataRow user in users.Rows)
                                            {
                                                string mobile = user[0].ToString();
                                                string key = "out_area_alarm_" + mobile + "_" + vehicleID;
                                                string code_value = cache.Get<string>(key);
                                                if (string.IsNullOrEmpty(code_value))
                                                {
                                                    bool sendResult = sim.SendAlarm(mobile, gpsNum, name, type);
                                                    if (sendResult)
                                                    {
                                                        Logger.Warn(string.Format("车辆{0}，{1}发生{2}报警，发送给{3}成功。", name, gpsNum, type, mobile));
                                                        DateTime dt = DateTime.Now.AddMinutes(20);
                                                        cache.Set(key, 1, dt - DateTime.Now);
                                                    }
                                                    else
                                                    {
                                                        Logger.Warn(string.Format("车辆{0}，{1}发生{2}报警，发送给{3}失败。", name, gpsNum, type, mobile));
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.Warn(string.Format("车辆{0}，{1}发生{2}报警，发送给{3}未超过1小时。", name, gpsNum, type, mobile));
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
                    }
                    catch
                    {
                        if (cache != null)
                        {
                            cache.Dispose();
                        }
                        Logger.Info("车辆越界报警发送短信失败");
                    }
                }
            }
            catch
            {
                if (cache != null)
                {
                    cache.Dispose();
                }
                Logger.Info("车辆越界报警发送短信失败");
            }
        }
    }

}