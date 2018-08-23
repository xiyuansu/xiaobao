using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetCache;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;

namespace YR.Web.api.xacloud.service
{
    /// <summary>
    /// 告警信息
    /// {"alarm":11,"sign":"53bed2367a0cd22bb27ae6a93816d3a3","cmd":"alarm","time":1499826311929,"carId":"865067025292346"}
    /// alarm:1(出围栏告警)，2(入围栏告警),3(振动告警),4(低电压告警),5(严重低电压告警),6(断电告警),7(电门打开通知),8(电门关闭通知),9(后座锁打开通知),10(后座锁关闭通知),11(电池恢复接通报警)
    /// </summary>
    public class AlarmAction : IApiAction
    {
        private static Log Logger = LogFactory.GetLogger(typeof(AlarmAction));

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.Code ="0" ;
            resp.Message ="success";
            string jsonData = params_ht["json"].ToString();
            ICache cache = null;
            try
            {
                dynamic jsonObj = DynamicJson.Parse(jsonData);
                string carId = "";
                if (jsonObj.IsDefined("carId"))
                {
                    carId = jsonObj.carId;
                }
                double alarm = 0;
                if (jsonObj.IsDefined("alarm"))
                {
                    alarm = jsonObj.alarm;
                }
                Logger.Debug("告警信息：" + carId + "，参数：" + jsonData);
                /*
                VehicleManager vm = new VehicleManager();
                VehicleAlarmManager alarmManager = new VehicleAlarmManager();
                Hashtable vehicle_ht = vm.GetVehicleIByGPSNum(carId);
                if (alarm == 6&& vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                {
                    string cacheKey = "Alarm_" + carId + "_" + alarm;
                    cache = CacheFactory.GetCache();
                    if (!string.IsNullOrEmpty(cache.Get<string>(cacheKey)))
                    {
                        cache.Dispose();
                        Logger.Error("告警信息20秒内，" + carId);
                        resp.Code = "1";
                        resp.Message = "20秒内过滤";
                        return resp;
                    }
                    int alarmType = 0;
                    //if (alarm == 3)
                    //{
                    //    振动
                    //    alarmType = 7;
                    //}
                    if (alarm == 6)
                    {
                        //断电
                        alarmType = 2;
                    }
                    string vid = vehicle_ht["ID"].ToString();
                    Hashtable ht = new Hashtable();
                    ht["ID"] = Guid.NewGuid().ToString();
                    ht["VehicleID"] = vid;
                    ht["IMEI"] = carId;
                    ht["AlarmType"] = alarmType;
                    ht["AlarmTime"] = DateTime.Now;
                    ht["AlarmStatus"] = 0;
                    ht["CreateTime"] = DateTime.Now;

                    bool result = false;
                    if (ht.Keys.Count > 0)
                    {
                        result = alarmManager.AddOrEdit(ht, null);
                        if (result)
                        {
                            DateTime dt = DateTime.Now.AddSeconds(20);
                            cache.Set(cacheKey, carId, dt - DateTime.Now);
                            cache.Dispose();
                        }
                    }
                    resp.Code = result ? "0" : "1";
                    resp.Message = result ? "success" : "fail";
                }*/
                return resp;
            }
            catch (Exception e)
            {
                if (cache != null)
                {
                    cache.Dispose();
                }
                Logger.Error("告警信息异常：" + e.Message + "，参数:" + jsonData);
                resp.Code = "1";
                resp.Message = e.Message;
                return resp;
            }
        }
    }
}