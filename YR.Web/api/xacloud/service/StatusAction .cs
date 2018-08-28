using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using YR.Common.DotNetCache;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;

namespace YR.Web.api.xacloud.service
{
    /// <summary>
    /// 状态信息
    /// {"distance":6315.7,"data":[{"acc":"1","wheel":"0","latitude":39.97863006591797,"locationType":0,"gpsTime":1498880904,"speed":0,"seat":"1","defend":"0","course":346,"power":"1","longitude":116.38182067871094}],"sign":"ae65f09315d08bf2011c64d8b86764eb","cmd":"status","time":1498880918082,"carId":"867717038875796"}
    /// </summary>
    public class StatusAction : IApiAction
    {
        static private int ThreadNumber = 20;
        static private Thread[] ThreadArray = new Thread[ThreadNumber];
        private static Log Logger = LogFactory.GetLogger(typeof(StatusAction));

        public ApiResp Execute(Hashtable params_ht)
        {
            int start = DateTime.Now.Millisecond;
            ApiResp resp = new ApiResp();
            ICache cache = null;
            try
            {
                cache = CacheFactory.GetCache();
                string jsonData = params_ht["json"].ToString();
                dynamic jsonObj = DynamicJson.Parse(jsonData);

                string carId = "";
                if (jsonObj.IsDefined("carId"))
                {
                    carId = jsonObj.carId;
                }
                double distance = 0.00;
                if (jsonObj.IsDefined("distance"))
                {
                    distance = jsonObj.distance;
                }

                if (jsonObj.IsDefined("data"))
                {
                    foreach (var item in jsonObj.data)
                    {
                        string acc = "";
                        if (item.IsDefined("acc"))
                        {
                            acc = item.acc;
                        }
                        double longitude = 0.00;
                        if (item.IsDefined("longitude"))
                        {
                            longitude = item.longitude;
                        }
                        double latitude = 0.00;
                        if (item.IsDefined("latitude"))
                        {
                            latitude = item.latitude;
                        }
                        double speed = 0.00;
                        if (item.IsDefined("speed"))
                        {
                            speed = item.speed;
                        }
                        string defend = "";
                        if (item.IsDefined("defend"))
                        {
                            defend = item.defend;
                        }
                        VehicleManager vm = new VehicleManager();
                        Hashtable vehicle_ht = vm.GetVehicleInfoByGPSNum(carId);
                        if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                        {
                            string vid = vehicle_ht["ID"].ToString();
                            //车辆使用状态 1空闲,2预约中,3客户使用中,4运维操作中
                            string useState = SiteHelper.GetHashTableValueByKey(vehicle_ht, "UseState");
                            string strLastUpdateTime = SiteHelper.GetHashTableValueByKey(vehicle_ht, "LastUpdateTime");
                            int diffSecond = 5;
                            if (!string.IsNullOrEmpty(strLastUpdateTime))
                            {
                                DateTime lastUpdateTime = DateTime.Parse(strLastUpdateTime);
                                diffSecond = SiteHelper.ExecDateDiffSecond(lastUpdateTime, DateTime.Now);
                            }
                            if (diffSecond > 3)
                            {
                                Hashtable ht = new Hashtable();
                                //车辆开关状态 LockState ,开:unlock，关:lock 
                                //acc =1 &&  defend =0  启动 unlock
                                if (acc.Equals("1") && defend.Equals("0"))
                                {
                                    ht["LockState"] = "unlock";
                                }
                                //defend =1  设防  lock
                                if (defend.Equals("1"))
                                {
                                    ht["LockState"] = "lock";
                                }
                                if (longitude > 0)
                                {
                                    ht["Longitude"] = longitude;
                                }
                                if (latitude > 0)
                                {
                                    ht["Latitude"] = latitude;
                                }
                                ht["CurrentSpeed"] = speed;
                                ht["Mileage"] = distance / 1000.00;
                                ht["GPSTime"] = DateTime.Now;
                                ht["LastUpdateTime"] = DateTime.Now;
                                bool result = false;
                                if (ht.Keys.Count > 0)
                                {
                                    result = vm.AddOrEditVehicleInfo(ht, vid);
                                }
                                string cacheValue;
                                Hashtable htAlarm = new Hashtable();
                                //超速
                                if (speed > 30)
                                {
                                    int count = 1;
                                    string overSpeedKey = "over_speed_" + carId;
                                    cacheValue = cache.Get<string>(overSpeedKey);
                                    if (string.IsNullOrEmpty(cacheValue))
                                    {
                                        DateTime dt = DateTime.Now.AddMinutes(5);
                                        cache.Set(overSpeedKey, count + "," + dt.ToString("yyyy-MM-dd HH:mm:ss"), dt - DateTime.Now);
                                    }
                                    else
                                    {
                                        if (cacheValue.IndexOf(",") > 0)
                                        {
                                            string[] countValue = cacheValue.Split(',');
                                            if (!string.IsNullOrEmpty(countValue[0]) && !string.IsNullOrEmpty(countValue[1]))
                                            {
                                                int.TryParse(countValue[0], out count);
                                                count += 1;
                                                if (count >= 10)
                                                {
                                                    Logger.Warn("超速报警," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                                    VehicleAlarmManager alarmManager = new VehicleAlarmManager();
                                                    htAlarm["ID"] = Guid.NewGuid().ToString();
                                                    htAlarm["VehicleID"] = vid;
                                                    htAlarm["IMEI"] = carId;
                                                    htAlarm["AlarmType"] = 6;
                                                    htAlarm["Speed"] = speed;
                                                    htAlarm["AlarmTime"] = DateTime.Now;
                                                    htAlarm["AlarmStatus"] = 0;
                                                    htAlarm["CreateTime"] = DateTime.Now;
                                                    if (alarmManager.AddOrEdit(htAlarm, null))
                                                    {
                                                        Logger.Warn("超速报警记录添加成功," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                                    }
                                                    else
                                                    {
                                                        Logger.Warn("超速报警记录添加失败," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                                    }
                                                }
                                                else
                                                {
                                                    DateTime lastTime = Convert.ToDateTime(countValue[1]);
                                                    TimeSpan timeSpan = lastTime - DateTime.Now;
                                                    if (timeSpan.Seconds > 1)
                                                    {
                                                        cache.Set(overSpeedKey, count + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), timeSpan);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (speed >= 5 && "1".Equals(useState))
                                    {
                                        int count = 1;
                                        string moveKey = "no_order_move_" + carId;
                                        cacheValue = cache.Get<string>(moveKey);
                                        if (string.IsNullOrEmpty(cacheValue))
                                        {
                                            DateTime dt = DateTime.Now.AddMinutes(5);
                                            cache.Set(moveKey, count + "," + dt.ToString("yyyy-MM-dd HH:mm:ss"), dt - DateTime.Now);
                                        }
                                        else
                                        {
                                            if (cacheValue.IndexOf(",") > 0)
                                            {
                                                string[] countValue = cacheValue.Split(',');
                                                if (!string.IsNullOrEmpty(countValue[0]) && !string.IsNullOrEmpty(countValue[1]))
                                                {
                                                    int.TryParse(countValue[0], out count);
                                                    count += 1;
                                                    if (count >= 10)
                                                    {
                                                        Logger.Warn("无单移动报警," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc + ",useState=" + useState);
                                                        VehicleAlarmManager alarmManager = new VehicleAlarmManager();
                                                        htAlarm["ID"] = Guid.NewGuid().ToString();
                                                        htAlarm["VehicleID"] = vid;
                                                        htAlarm["IMEI"] = carId;
                                                        htAlarm["AlarmType"] = 1;
                                                        htAlarm["Speed"] = speed;
                                                        htAlarm["AlarmTime"] = DateTime.Now;
                                                        htAlarm["AlarmStatus"] = 0;
                                                        htAlarm["CreateTime"] = DateTime.Now;
                                                        if (alarmManager.AddOrEdit(htAlarm, null))
                                                        {
                                                            Logger.Warn("无单移动报警记录添加成功," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                                        }
                                                        else
                                                        {
                                                            Logger.Warn("无单移动报警记录添加失败," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    DateTime lastTime = Convert.ToDateTime(countValue[1]);
                                                    TimeSpan timeSpan = lastTime - DateTime.Now;
                                                    if (timeSpan.Seconds > 1)
                                                    {
                                                        cache.Set(moveKey, count + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), timeSpan);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                string outAreaKey = "out_area_" + carId;
                                cacheValue = cache.Get<string>(outAreaKey);
                                if (string.IsNullOrEmpty(cacheValue) && latitude > 0 && longitude > 0)
                                {
                                    string serviceAreaKey = "Service_Area_" + vehicle_ht["CITYID"].ToString();
                                    string coordinates = cache.Get<string>(serviceAreaKey);
                                    if (string.IsNullOrEmpty(coordinates))
                                    {
                                        ServiceAreaManager areaManager = new ServiceAreaManager();
                                        Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(vid);
                                        if (area_ht != null && area_ht.Keys.Count > 0)
                                        {
                                            coordinates = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates");
                                            if (!string.IsNullOrEmpty(coordinates))
                                            {
                                                DateTime timeSpan = DateTime.Now.AddDays(10);
                                                cache.Set(serviceAreaKey, coordinates, timeSpan - DateTime.Now);
                                            }
                                        }
                                    }
                                    List<LatLng> area_pts = new List<LatLng>();
                                    if (!string.IsNullOrEmpty(coordinates))
                                    {
                                        foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                            LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                            area_pts.Add(pt);
                                        }
                                        LatLng vehicle_pt = new LatLng(latitude, longitude);
                                        //车辆GPS坐标转高德坐标
                                        vehicle_pt = SiteHelper.GPSToGCJ02(longitude.ToString(), latitude.ToString());
                                        vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                                        bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                                        if (!isInPoly)
                                        {
                                            Logger.Warn("越界报警," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                            VehicleAlarmManager alarmManager = new VehicleAlarmManager();
                                            htAlarm["ID"] = Guid.NewGuid().ToString();
                                            htAlarm["VehicleID"] = vid;
                                            htAlarm["IMEI"] = carId;
                                            htAlarm["AlarmType"] = 4;
                                            htAlarm["Speed"] = speed;
                                            htAlarm["AlarmTime"] = DateTime.Now;
                                            htAlarm["AlarmStatus"] = 0;
                                            htAlarm["CreateTime"] = DateTime.Now;
                                            bool alarmResult = alarmManager.AddOrEdit(htAlarm, null);
                                            if (alarmResult)
                                            {
                                                //越界20分钟内有效
                                                DateTime dt = DateTime.Now.AddMinutes(20);
                                                cache.Set(outAreaKey, carId, dt - DateTime.Now);
                                                Logger.Warn("越界报警记录添加成功," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                            }
                                            else
                                            {
                                                Logger.Warn("越界报警记录添加失败," + vid + "," + carId + ",speed=" + speed + ",defend =" + defend + ",acc=" + acc);
                                            }
                                        }
                                    }
                                }
                                int end = DateTime.Now.Millisecond;
                                Logger.Warn("小安盒子接口网关上报状态信息数据,耗时" + (end - start) + "ms," + vid + "," + carId + ",defend = " + defend + ",acc=" + acc + ",LockState=" + ht["LockState"]);
                                resp.Code = result ? "0" : "1";
                                resp.Message = result ? "success" : "fail";
                            }
                            else
                            {
                                resp.Code = "3";
                            }
                        }
                    }
                }
                if (cache != null)
                {
                    cache.Dispose();
                }
                return resp;
            }
            catch (Exception e)
            {
                Logger.Error("盒子上报状态信息报错:" + e);
                string message = e.Message;
                if (cache != null)
                {
                    cache.Dispose();
                }
                resp.Code = "1";
                resp.Message = "fail";
                return resp;
            }
        }
    }
}