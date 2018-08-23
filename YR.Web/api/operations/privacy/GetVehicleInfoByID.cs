using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Data;
using YR.Web.api.api_class;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 获取单个车辆信息
    /// </summary>
    public class GetVehicleInfoByID : IApiAction2
    {
        private string vehicleID = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["VehicleID"] == null || res["VehicleID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                vehicleID = res["VehicleID"].ToString().Trim();
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
                Hashtable vehicle = vm.GetVehicleInfoByIDORNumber(vehicleID);
                OrdersManager om = new OrdersManager();
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
                        latlng = SiteHelper.TransformFromWGSToGCJ(latlng);
                        if (latlng != null)
                        {
                            vehicle["LATITUDE"] = latlng.latitude;
                            vehicle["LONGITUDE"] = latlng.longitude;
                            if (vehicle["ADDRESS"].ToString().Length <= 0)
                            {
                                vehicle["ADDRESS"] = SiteHelper.GetLocationByGPS(vehicle["LONGITUDE"].ToString(), vehicle["LATITUDE"].ToString(), "gcj02ll");
                            }
                        }
                    }
                    decimal electricity = 0;
                    decimal.TryParse(vehicle["ELECTRICITY"].ToString(), out electricity);
                    int dianliang = Decimal.ToInt32(electricity);
                    vehicle["ELECTRICITY"] = dianliang + "%";

                    vehicle["AbnormalState"] = 0;
                    Hashtable abht = null;// vm.GetAbnormalVehicle(vehicle["ID"].ToString());
                    if (abht != null && abht.Keys.Count > 0)
                    {
                        vehicle["AbnormalState"] = 1;
                        vehicle["AlarmType"] = SiteHelper.GetHashTableValueByKey(abht, "AlarmType");
                        vehicle["AlarmTime"] = SiteHelper.GetHashTableValueByKey(abht, "AlarmTime");
                    }

                    vehicle["ChargeBeginTime"] = "";
                    Hashtable chargeht = vm.GetVehicleChargingInfo(vehicle["ID"].ToString());
                    if (chargeht != null && chargeht.Keys.Count > 0)
                        vehicle["ChargeBeginTime"] = chargeht["BEGINTIME"].ToString();

                    Hashtable order_ht = om.GetLatestUserByVehicleID(vehicle["ID"].ToString());
                    if (order_ht != null && order_ht.Keys.Count > 0)
                    {
                        vehicle["OrderUserName"] = order_ht["REALNAME"];
                        vehicle["OrderUserTel"] = order_ht["BINDPHONE"];
                        //vehicle["HeadPortrait"] = SiteHelper.GetFileServer() + order_ht["HEADPORTRAIT"];
                        vehicle["HeadPortrait"] = "";
                        vehicle["OrderState"] = order_ht["ORDERSTATE"];
                        vehicle["UserType"] = order_ht["USERTYPE"];
                        vehicle["OrderTime"] = order_ht["CREATETIME"];
                    }
                    else
                    {
                        vehicle["OrderUserName"] = "";
                        vehicle["OrderUserTel"] = "";
                        vehicle["HeadPortrait"] = "";
                        vehicle["OrderState"] = "";
                        vehicle["UserType"] = "";
                        vehicle["OrderTime"] = "";
                    }
                    return SiteHelper.GetJsonFromHashTable(vehicle, "success", "获取数据成功", "VehicleInfo");
                }
            }
        }
    }
}