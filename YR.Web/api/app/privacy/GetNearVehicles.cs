using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using Newtonsoft.Json.Linq;
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
    /// 得到附近的车辆
    /// </summary>
    public class GetNearVehicles : IApiAction2
    {
        private double km = 5;

        private int pageNum = 20;

        private int currentPage = 1;

        private string weidu = string.Empty;

        private string jingdu = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            /* if (!string.IsNullOrEmpty(SiteHelper.GetAppsetString("NearVehicleKM")))
             {
                 km = double.Parse(SiteHelper.GetAppsetString("NearVehicleKM"));
             }
             if (!string.IsNullOrEmpty(SiteHelper.GetAppsetString("PageNum")))
             {
                 pageNum = int.Parse(SiteHelper.GetAppsetString("PageNum"));
             }*/
            if (res["Latitude"] == null ||
                res["Longitude"] == null ||
                res["PageNum"] == null ||
                res["CurrentPage"] == null ||
                res["Latitude"].ToString().Trim().Length <= 0 ||
                res["Longitude"].ToString().Trim().Length <= 0 ||
                res["PageNum"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    currentPage = int.Parse(res["CurrentPage"].ToString().Trim());
                    weidu = res["Latitude"].ToString().Trim();
                    jingdu = res["Longitude"].ToString().Trim();
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }
                VehicleManager vm = new VehicleManager();
                DataTable vehicles = vm.GetNearVehicles(km, weidu, jingdu, currentPage, pageNum);
                if (vehicles == null || vehicles.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "亲，没有数据了");
                }
                else
                {
                    foreach (DataRow dr in vehicles.Rows)
                    {
                        decimal electricity = 0;
                        decimal.TryParse(dr["Electricity"].ToString(), out electricity);
                        int dianliang = Decimal.ToInt32(electricity);
                        dr["Electricity"] = dianliang + "%";
                        string lng = dr["Longitude"].ToString();
                        string lat = dr["Latitude"].ToString();
                        if (!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat))
                        {
                            LatLng latlng = new LatLng(double.Parse(lat), double.Parse(lng));
                            latlng = SiteHelper.TransformFromWGSToGCJ(latlng);
                            dr["Longitude"] = latlng.longitude;
                            dr["Latitude"] = latlng.latitude;
                        }
                    }
                    return JsonHelper.DataTableToJson("success", "获取数据成功", vehicles, "VehicleInfo");
                }
            }
        }
        
    }
}