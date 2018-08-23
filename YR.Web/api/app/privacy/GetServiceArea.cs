using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取当前所在位置城市的车辆运营区域
    /// </summary>
    public class GetServiceArea : IApiAction2
    {
        public string Execute(Hashtable res)
        {
            if (res["Longitude"] == null || res["Longitude"].ToString().Trim().Length <= 0 ||
                res["Latitude"] == null || res["Latitude"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                string longitude = res["Longitude"].ToString().Trim();
                string latitude = res["Latitude"].ToString().Trim();

                string citycode="",cityname="",districtcode="",districtname="";
                string address = SiteHelper.GetAddressInfoByGPS(longitude, latitude, out citycode, out cityname,out districtcode,out districtname);

                ObjectApiResp resp = new ObjectApiResp();
                resp.Code = "-1";
                resp.Message = "";

                ServiceAreaManager areaManager = new ServiceAreaManager();
                Hashtable ht= areaManager.GetServiceAreaByCityCode(districtcode);
                if (ht == null || ht.Keys.Count==0)
                {
                    ht = areaManager.GetServiceAreaByCityCode(citycode);
                }
                if (ht != null && ht.Keys.Count > 0)
                {
                    GetServiceAreaResponse data = new GetServiceAreaResponse();
                    data.city = cityname;
                    data.points = SiteHelper.GetHashTableValueByKey(ht, "Coordinates");
                    data.longitude = SiteHelper.GetHashTableValueByKey(ht, "Longitude");
                    data.latitude = SiteHelper.GetHashTableValueByKey(ht, "Latitude");
                    resp.Data = data;
                    resp.Code = "0";
                }

                return JsonConvert.SerializeObject(resp);
            }
        }
    }

    /// <summary>
    /// 接口GetServiceArea返回数据
    /// </summary>
    class GetServiceAreaResponse
    {
        public string city;
        public string points;
        public string longitude;
        public string latitude;
    }
}