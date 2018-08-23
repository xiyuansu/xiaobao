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
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取单个车辆信息
    /// </summary>
    public class GetVehicleInfoByID : IApiAction2
    {
        private string vehicleID = string.Empty;
        private string weidu = string.Empty;
        private string jingdu = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["VehicleID"] == null ||
                res["Latitude"] == null ||
                res["Longitude"] == null ||
                res["VehicleID"].ToString().Trim().Length <= 0 ||
                res["Latitude"].ToString().Trim().Length <= 0 ||
                res["Longitude"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                vehicleID = res["VehicleID"].ToString().Trim();
                weidu = res["Latitude"].ToString().Trim();
                jingdu = res["Longitude"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                SystemImagesManager sim = new SystemImagesManager();

                //如何扫码内容为url地址则从地址中取出车辆id
                if (vehicleID != null && (vehicleID.StartsWith("http://") || vehicleID.StartsWith("https://")))
                {
                    //http://api.qingniaokeji.cn/download/app.aspx?no=xxxxxxxxxxxxx
                    Uri uri = new Uri(vehicleID);
                    System.Collections.Specialized.NameValueCollection coll = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    vehicleID = coll.Get("no");
                }
                Hashtable vehicle = vm.GetVehicleInfoByID(vehicleID, weidu, jingdu);
                if (vehicle == null || vehicle.Keys.Count <= 0)
                {
                    vehicle = vm.GetVehicleInfoByName(vehicleID, weidu, jingdu);
                }

                string thumbImg = string.Empty;
                string detailsImg = string.Empty;
                if (vehicle == null || vehicle.Keys.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆数据为空");
                }
                else
                {
                    DataTable dt = sim.GetImagesByVehicleID(vehicleID);
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["Type"].ToString().Trim() == ((int)ImagesType.Thumbnail).ToString().Trim())
                            {
                                thumbImg += SiteHelper.GetFileServer() + dr["ImageUrl"].ToString().Trim() + ";";
                            }

                            if (dr["Type"].ToString().Trim() == ((int)ImagesType.Details).ToString().Trim())
                            {
                                detailsImg += SiteHelper.GetFileServer() + dr["ImageUrl"].ToString().Trim() + ";";
                            }
                        }
                    }
                    vehicle["THUMBIMG"] = thumbImg;
                    vehicle["DETAILSIMG"] = detailsImg;

                    string lng = vehicle["LONGITUDE"].ToString();
                    string lat = vehicle["LATITUDE"].ToString();
                    if (!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat))
                    {
                        LatLng latlng = new LatLng(double.Parse(lat), double.Parse(lng));
                        if (res["Client"] != null && "iOS" == res["Client"].ToString())
                        {
                            latlng = new LatLng(double.Parse(lat), double.Parse(lng));
                            latlng = SiteHelper.TransformFromWGSToGCJ(latlng);
                            vehicle["LONGITUDE"] = latlng.longitude;
                            vehicle["LATITUDE"] = latlng.latitude;
                        }
                        else
                        {
                            /*latlng = SiteHelper.GetBaiduByGPS(lat, lng);
                            if (latlng != null)
                            {
                                vehicle["LATITUDE"] = latlng.latitude;
                                vehicle["LONGITUDE"] = latlng.longitude;
                                if (vehicle["ADDRESS"].ToString().Length <= 0)
                                {
                                    vehicle["ADDRESS"] = SiteHelper.GetLocationByGPS(vehicle["LATITUDE"].ToString(), vehicle["LONGITUDE"].ToString());
                                }
                            }*/
                            latlng = new LatLng(double.Parse(lat), double.Parse(lng));
                            latlng = SiteHelper.TransformFromWGSToGCJ(latlng);
                            vehicle["LONGITUDE"] = latlng.longitude;
                            vehicle["LATITUDE"] = latlng.latitude;
                            string citycode = "", cityname = "";
                            vehicle["ADDRESS"] = SiteHelper.GetAddressInfoByGPS(latlng.longitude.ToString(), latlng.latitude.ToString(), out citycode, out cityname);
                        }
                    }
                    decimal electricity = 0;
                    decimal.TryParse(vehicle["ELECTRICITY"].ToString(), out electricity);
                    int dianliang = Decimal.ToInt32(electricity);
                    vehicle["ELECTRICITY"] = dianliang + "%";

                    vehicle["PRICESYSTEM"] = SiteHelper.GetFileServer() + "/AppPage/BusinessInfo.aspx?id=" + SiteHelper.GetHashTableValueByKey(vehicle, "ID") + "&type=" + BusinessType.Vehicle.GetHashCode() + "&field=" + BusinessFieldType.PRICESYSTEM.GetHashCode();
                    vehicle["VEHICLEREMARK"] = SiteHelper.GetFileServer() + "/AppPage/BusinessInfo.aspx?id=" + SiteHelper.GetHashTableValueByKey(vehicle, "ID") + "&type=" + BusinessType.Vehicle.GetHashCode() + "&field=" + BusinessFieldType.VEHICLEREMARK.GetHashCode();
                    vehicle["TRANSPORTINFORMATION"] = SiteHelper.GetFileServer() + "/AppPage/BusinessInfo.aspx?id=" + SiteHelper.GetHashTableValueByKey(vehicle, "ID") + "&type=" + BusinessType.Vehicle.GetHashCode() + "&field=" + BusinessFieldType.TRANSPORTINFORMATION.GetHashCode();
                    return SiteHelper.GetJsonFromHashTable(vehicle, "success", "获取数据成功", "VehicleInfo");
                }
            }
        }
    }
}