using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using cn.jpush.api;
using cn.jpush.api.push;
using cn.jpush.api.push.mode;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;
using YR.Common.DotNetConfig;
using YR.Common.DotNetLog;
using YR.Web.App_Code;

namespace YR.Web.jobs
{
    /// <summary>
    /// 还车通知，锁车超过指定时间后发送还车通知
    /// </summary>
    public class ReturnVehicleNotify : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(ReturnVehicleNotify));

        private String APPKEY = "";

        private String MASTERSECRET = "";

        /// <summary>
        /// 占用车辆超时还车通知
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                int minutes_timeout = 30;
                int.TryParse(context.JobDetail.JobDataMap.Get("minutes_timeout").ToString(),out minutes_timeout);
                int ios_production = 0;
                int.TryParse(context.JobDetail.JobDataMap.Get("ios_production").ToString(), out ios_production);

                List<string> saasList=ConfigHelper.GetSaasList();
                foreach(string saas in saasList)
                {
                    try
                    {
                        MyWorkerRequest.CreateHttpContext(saas, "", "");

                        APPKEY = SiteHelper.GetAppsetString("JPush_AppKey");
                        MASTERSECRET = SiteHelper.GetAppsetString("JPush_MasterSecret");

                        JPushClient pushClient = new JPushClient(APPKEY, MASTERSECRET);

                        OrdersManager om = new OrdersManager();
                        DataTable dt = om.GetVehicleReturnOrders(minutes_timeout);
                        foreach (DataRow dr in dt.Rows)
                        {
                            string clientid = string.Empty;
                            if (dr["PushID"] != DBNull.Value)
                                clientid = dr["PushID"].ToString();
                            string licenseNumber = dr["LicenseNumber"].ToString();
                            if (!string.IsNullOrEmpty(clientid))
                            {
                                PushMessage pushMessage = new PushMessage();
                                pushMessage.code = "0101";
                                pushMessage.message = "您当前占用的车辆已锁车停留较长时间，如不再使用请及时还车，避免不必要的浪费哟！";

                                PushPayload payload = new PushPayload();
                                payload.platform = Platform.all();
                                payload.audience = Audience.s_registrationId(new string[] { clientid });
                                payload.notification = new Notification().setAlert(pushMessage.message);
                                payload.message = Message.content(Newtonsoft.Json.JsonConvert.SerializeObject(pushMessage));
                                payload.options = new Options();
                                payload.options.time_to_live = 86400 * 10;
                                payload.options.apns_production = ios_production == 1 ? true : false;

                                MessageResult pushResult = pushClient.SendPush(payload);
                                if (pushResult.isResultOK())
                                    Logger.Info("车辆还车通知:" + pushResult + ",车辆：" + licenseNumber);
                                else
                                    Logger.Info("车辆还车通知失败");
                            }
                        }
                    }
                    catch
                    {
                        Logger.Info("车辆还车通知任务(ReturnVehicleNotify:"+saas+")失败");
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