using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using YR.Common.DotNetCode;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 判断是否在范围内
    /// </summary>
    public class InArea : IApiAction2
    {
        private string uid = string.Empty;

        private string userLng = string.Empty;

        private string userLat = string.Empty;

        public string Execute(Hashtable params_ht)
        {

            //EndLongitude	115.809219360352  EndLatitude  25.5647277832031
            //GPS 115.809219360352    25.5647277832031   115.779479980469    25.5972442626953
            string longitude = "115.809219360352";
            string latitude = "25.5647277832031";
            userLng = "115.779479980469";
            userLat = "25.5972442626953";

            List<LatLng> area_pts = new List<LatLng>();
            string coordinates = "115.765816,25.58452;115.76756,25.592613;115.769011,25.602102;115.77077,25.608795;115.787668,25.606627;115.791111,25.616351;115.80404,25.620546;115.808559,25.618937;115.81353,25.617623;115.820726,25.616464;115.83187,25.614221;115.834108,25.611296;115.832017,25.601225;115.814038,25.576651;115.810133,25.571696;115.809087,25.570536;115.804249,25.571603;115.801075,25.5725;115.798363,25.573333";
            foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                area_pts.Add(pt);
            }
            LatLng vehicle_pt = new LatLng(double.Parse(latitude), double.Parse(longitude));
            vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
            vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
            bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
            bool isInPoly2 = false;//用户坐标是否在区域内
            LatLng user_pt = new LatLng(double.Parse(userLat), double.Parse(userLng));
            user_pt = SiteHelper.GPSToGCJ02(userLng, userLat);
            user_pt = new LatLng(Math.Round(user_pt.latitude, 6), Math.Round(user_pt.longitude, 6));
            isInPoly2 = SiteHelper.IsPtInPoly(user_pt, area_pts);
            if (isInPoly)
            {
                if (isInPoly2)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "1在范围内,2在范围内", "OrderInfo");
                }
                else
                {

                    return SiteHelper.GetJsonFromHashTable(null, "success", "1在范围内,2在范围外", "OrderInfo");
                }
            }
            else
            {
                if (isInPoly2)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "1在范围外,2在范围内", "OrderInfo");
                }
                else
                {

                    return SiteHelper.GetJsonFromHashTable(null, "success", "1在范围外,2在范围外", "OrderInfo");
                }
            }
        }

    }
}