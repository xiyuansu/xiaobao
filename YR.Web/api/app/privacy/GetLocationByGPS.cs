using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 根据GPS信息获取地理位置
    /// </summary>
    public class GetLocationByGPS : IApiAction2
    {
        private string weidu = string.Empty;
        private string jingdu = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["Latitude"] == null ||
                res["Longitude"] == null ||
               res["Latitude"].ToString().Trim().Length <= 0 ||
               res["Longitude"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                weidu = res["Latitude"].ToString().Trim();
                jingdu = res["Longitude"].ToString().Trim();
                Hashtable location = new Hashtable();
                location["ADDRESS"] = SiteHelper.GetLocationByGPS(weidu, jingdu);
                if (res["Client"] != null && "iOS" == res["Client"].ToString())
                {
                    LatLng latlng = SiteHelper.GetBaiduByGPS(jingdu, weidu);
                    location["ADDRESS"] = SiteHelper.GetLocationByGPS(latlng.longitude.ToString(), latlng.latitude.ToString(),"gcj02ll");
                }
                location["LATITUDE"] = weidu;
                location["LONGITUDE"] = jingdu;
                return SiteHelper.GetJsonFromHashTable(location, "success", "获取数据成功", "LocationInfo");
            }
        }
    }
}