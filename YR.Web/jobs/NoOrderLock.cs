using Asiasofti.SmartVehicle.Manager;
using Quartz;
using System.Data;
using YR.Common.DotNetConfig;
using YR.Common.DotNetLog;
using YR.Web.App_Code;

namespace YR.Web.jobs
{
    /// <summary>
    /// 定时检测无单车并进行断电锁车
    /// </summary>
    public class NoOrderLock : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(CancelOrders));

        public void Execute(IJobExecutionContext context)
        {
            //ICache Cache = null;
            try
            {
                foreach (string saas in ConfigHelper.GetSaasList())
                {
                    try
                    {
                        MyWorkerRequest.CreateHttpContext(saas, "", "");
                        //Cache = CacheFactory.GetCache();
                        VehicleManager vm = new VehicleManager();
                        DataTable dt = vm.GetNoOrderUnlockVehicles();
                        foreach (DataRow row in dt.Rows)
                        {
                            string vid = row["ID"].ToString();
                            string name = row["Name"].ToString();
                            string carId = row["VehicleGPSNum"].ToString();

                            /*string keyid = "vehicle_" + carId;
                            string json_car=Cache.Get<string>(keyid);
                            string acc = "";
                            if (!string.IsNullOrEmpty(json_car))
                            {
                                dynamic jsonObj = DynamicJson.Parse(json_car);
                                if (jsonObj.IsDefined("data"))
                                {
                                    foreach (var item in jsonObj.data)
                                    {
                                        if (item.IsDefined("acc"))
                                            acc = item.acc;
                                    }
                                }
                            }*/

                            double speed = 0.00;
                            double.TryParse(row["CurrentSpeed"].ToString(), out speed);
                            //if (acc =="1"){
                            if (vm.CloseVehicle(vid))
                            {
                                Logger.Info("无单锁车成功,车辆号:" + name);
                            }
                            else
                            {
                                Logger.Info("无单锁车失败,车辆号:" + name);
                            }
                            //}
                        }
                        //Cache.Dispose();
                    }
                    catch
                    {
                        /*if (Cache != null)
                        {
                            Cache.Dispose();
                        }*/
                        continue;
                    }
                }
            }
            catch
            {
                /*if (Cache != null)
                {
                    Cache.Dispose();
                }*/
                Logger.Info("无单车进行断电锁车(NoOrderLock)失败");
            }
        }
    }
}