using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;
using YR.Web.api.api_class;

namespace YR.Web.api.ydcloud.service
{
    /// <summary>
    /// 状态信息,设备定期的上报车辆的位置信息和相关的工作状态信息
    /// </summary>
    public class StatusAction : IApiAction
    {
        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            try
            {
                string jsonData = params_ht["json"].ToString();
                dynamic jsonObj = DynamicJson.Parse(jsonData);

                string carId = "";
                if (jsonObj.IsDefined("carId"))
                    carId = jsonObj.carId;//车辆ID
                string latitude = "";
                if (jsonObj.IsDefined("body.latitude"))
                    latitude = jsonObj.body.latitude;//纬度
                string longitude = "";
                if (jsonObj.IsDefined("body.longitude"))
                    longitude = jsonObj.body.longitude;//经度
                string elevation = "";
                if (jsonObj.IsDefined("body.elevation"))
                    elevation = jsonObj.body.elevation;//海拔
                string speed = "";
                if (jsonObj.IsDefined("body.speed"))
                    speed = jsonObj.body.speed;//车速
                string direction = "";
                if (jsonObj.IsDefined("body.direction"))
                    direction = jsonObj.body.direction;//方向
                string time = "";
                if (jsonObj.IsDefined("body.time"))
                    time = jsonObj.body.time;//时间
                string surplusPercent = "";
                if (jsonObj.IsDefined("body.surplusPercent"))
                    surplusPercent = jsonObj.body.surplusPercent;//剩余电量百分比
                string surplusDistance = "";
                if (jsonObj.IsDefined("body.surplusDistance"))
                    surplusDistance = jsonObj.body.surplusDistance;//剩余续航里程 
                string distance = "";
                if (jsonObj.IsDefined("body.distance"))
                    distance = jsonObj.body.distance;//总行驶里程

                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByGPSNum(carId);
                if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                {
                    string vid = vehicle_ht["ID"].ToString();
                    Hashtable ht = new Hashtable();
                    if (!string.IsNullOrEmpty(longitude))
                        ht["Longitude"] = longitude;
                    if (!string.IsNullOrEmpty(latitude))
                        ht["Latitude"] = latitude;
                    if (!string.IsNullOrEmpty(speed))
                        ht["CurrentSpeed"] = speed;
                    if (!string.IsNullOrEmpty(time))
                        ht["GPSTime"] = DateTime.Now;
                    if (!string.IsNullOrEmpty(surplusPercent))
                        ht["Electricity"] = surplusPercent;
                    if (!string.IsNullOrEmpty(surplusDistance))
                        ht["ExpectRange"] = surplusDistance;
                    if (!string.IsNullOrEmpty(distance))
                        ht["Mileage"] = distance;

                    bool result = false;
                    if (ht.Keys.Count > 0)
                        result = vm.AddOrEditVehicleInfo(ht, vid);
                    resp.Code = result ? "0" : "1";
                }
                return resp;
            }
            catch
            {
                resp.Code = "1";
                return resp;
            }
        }
    }
}