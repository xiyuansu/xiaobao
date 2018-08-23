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

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 获取当前网点用户占车信息
    /// </summary>
    public class GetCurrentVehicle : IApiAction2
    {
        private string uid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                SystemImagesManager sim = new SystemImagesManager();
                OPUserManager opUserManager = new OPUserManager();
                Hashtable vehicle = opUserManager.GetVehicleInfoByOperator(uid);
                OrdersManager om = new OrdersManager();
                string thumbImg = string.Empty;
                string detailsImg = string.Empty;
                if (vehicle == null || vehicle.Keys.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆数据为空");
                }
                else
                {

                    DataTable dt = sim.GetImagesByVehicleID(vehicle["ID"].ToString());
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

                    Hashtable order_ht = om.GetLatestUserByVehicleID(vehicle["ID"].ToString());
                    if (order_ht != null && order_ht.Keys.Count > 0)
                    {
                        vehicle["OrderUserName"] = order_ht["REALNAME"];
                        vehicle["OrderUserTel"] = order_ht["BINDPHONE"];
                        vehicle["HeadPortrait"] = SiteHelper.GetFileServer() + order_ht["HEADPORTRAIT"];
                        vehicle["OrderState"] = order_ht["ORDERSTATE"];
                        vehicle["UserType"] = order_ht["USERTYPE"];
                    }
                    else
                    {
                        vehicle["OrderUserName"] = "";
                        vehicle["OrderUserTel"] = "";
                        vehicle["HeadPortrait"] = "";
                        vehicle["OrderState"] = "";
                        vehicle["UserType"] = "";
                    }

                    return SiteHelper.GetJsonFromHashTable(vehicle, "success", "获取数据成功", "GetCurrentVehicle");
                }
            }
        }
    }
}