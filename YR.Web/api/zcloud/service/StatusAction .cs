using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Web.api.api_class;

namespace YR.Web.api.zcloud.service
{
    /// <summary>
    /// 位置信息,设备定期的上报车辆的位置信息和相关的工作状态信息
    /// </summary>
    public class StatusAction : IApiAction
    {
        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            try
            {
                string carId = SiteHelper.GetHashTableValueByKey(params_ht,"carId");//车辆ID
                string latitude = SiteHelper.GetHashTableValueByKey(params_ht,"latitude");//纬度
                string longitude = SiteHelper.GetHashTableValueByKey(params_ht,"longitude");//经度
                string elevation = SiteHelper.GetHashTableValueByKey(params_ht,"elevation");//海拔
                string speed = SiteHelper.GetHashTableValueByKey(params_ht,"speed");//车速
                string direction = SiteHelper.GetHashTableValueByKey(params_ht,"direction");//方向
                string time = SiteHelper.GetHashTableValueByKey(params_ht,"time");//时间
                string surplusPercent = SiteHelper.GetHashTableValueByKey(params_ht,"surplusPercent");//剩余电量百分比
                string surplusDistance = SiteHelper.GetHashTableValueByKey(params_ht,"surplusDistance");//剩余续航里程 
                string distance = SiteHelper.GetHashTableValueByKey(params_ht,"distance");//总行驶里程

                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByGPSNum(carId);
                if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                {
                    string vid = vehicle_ht["ID"].ToString();
                    Hashtable ht = new Hashtable();
                    if (!string.IsNullOrEmpty(longitude))
                        ht["Longitude"] = string.Format("{0}", decimal.Parse(longitude) / 1000000);
                    if (!string.IsNullOrEmpty(latitude))
                        ht["Latitude"] = string.Format("{0}", decimal.Parse(latitude) / 1000000);
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
                    bool result = vm.AddOrEditVehicleInfo(ht, vid);
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