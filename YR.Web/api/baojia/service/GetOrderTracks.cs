using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace YR.Web.api.baojia.service
{
    /// <summary>
    /// 订单轨迹
    /// </summary>
    public class GetOrderTracks : IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();
            string order_no = params_ht["order_no"].ToString();

            BaoJiaManager mgr = new BaoJiaManager();
            Hashtable order_ht=mgr.GetOrderInfo(order_no);
            if (order_ht != null && order_ht.Keys.Count > 0)
            {
                ServiceAreaManager areaManager = new ServiceAreaManager();

                string vehicleid = SiteHelper.GetHashTableValueByKey(order_ht, "VehicleID");
                string orderid = SiteHelper.GetHashTableValueByKey(order_ht, "ID");

                GetOrderTracks_Data data = new GetOrderTracks_Data();

                string returnLocType=SiteHelper.GetHashTableValueByKey(order_ht, "ReturnLocType");
                if(returnLocType=="01")
                {
                    data.return_location_address_type = "3";
                    data.return_location_address_msg = "网点还车";

                    string toParking = SiteHelper.GetHashTableValueByKey(order_ht, "ToParking");
                    if (!string.IsNullOrEmpty(toParking))
                    {
                        Hashtable parking_ht= areaManager.GetInfoByID(toParking);
                        if (parking_ht != null && parking_ht.Keys.Count > 0)
                        {
                            data.return_station_points = SiteHelper.GetHashTableValueByKey(parking_ht, "Coordinates").Replace(";", "|");
                            data.return_station_center = SiteHelper.GetHashTableValueByKey(parking_ht, "Longitude") + "," + SiteHelper.GetHashTableValueByKey(parking_ht, "Latitude");
                        }
                    }
                }
                else if (returnLocType == "02" || returnLocType == "03")
                {
                    data.return_location_address_type = "1";
                    data.return_location_address_msg = "行驶区域内网点外还车";
                }
                else if (returnLocType == "04")
                {
                    data.return_location_address_type = "2";
                    data.return_location_address_msg = "行驶区域外还车";

                    Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(vehicleid);
                    if (area_ht != null && area_ht.Keys.Count > 0)
                        data.return_xianzhi_points = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates").Replace(";", "|");
                }

                VehicleManager vm = new VehicleManager();
                DataTable dt = vm.GetVehicleTrace(vehicleid, orderid, null, null);
                if (dt != null)
                {
                    GetOrderTracks_Data Data = new GetOrderTracks_Data();

                    data.return_location_address_msg="";
                    data.line_points="";
                    List<LatLng> pts=new List<LatLng>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        double lng = double.Parse(dr["AMAP_longitude"].ToString());
                        double lat = double.Parse(dr["AMAP_latitude"].ToString());
                        pts.Add(new LatLng(lat,lng));

                        if(data.line_points.Length>0 && !data.line_points.EndsWith("|"))
                            data.line_points+="|";
                        data.line_points+=lng+","+lat;
                    }
                    if(pts.Count>0)
                    {
                        data.start_lng=pts[0].longitude;
                        data.star_lat=pts[0].latitude;

                        data.end_lng=pts[pts.Count-1].longitude;
                        data.end_lat=pts[pts.Count-1].latitude;
                    }
                    else
                    {
                        LatLng latlng = new LatLng(double.Parse(SiteHelper.GetHashTableValueByKey(order_ht, "EndLatitude")), double.Parse(SiteHelper.GetHashTableValueByKey(order_ht, "EndLongitude")));
                        latlng = SiteHelper.TransformFromWGSToGCJ(latlng);
                        data.start_lng = latlng.longitude;
                        data.star_lat = latlng.latitude;
                        data.end_lng = data.start_lng;
                        data.end_lat = data.star_lat;
                    }

                    resp.code = "0";
                    resp.msg = "成功";
                    resp.data = data;
                }
                else
                {
                    resp.code = "-1";
                    resp.msg = "未找到停车网点";
                }
            }
            else
            {
                resp.code = "-1";
                resp.msg = "未找到相关订单";
            }

            return resp;
        }
    }

    public class GetOrderTracks_Data
    {
        public string return_location_address_type;

        public string return_location_address_msg;

        public double start_lng;

        public double star_lat;

        public double end_lng;

        public double end_lat;

        public string line_points;

        public string return_station_points;

        public string return_station_center;

        public string return_xianzhi_points;
    }

}