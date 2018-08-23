using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// 得到附近的还车网点
    /// </summary>
    public class GetNearParking : IApiAction2
    {
        private double km = 10;

        private int pageNum = 100;

        private int currentPage = 1;

        public string Execute(Hashtable res)
        {
            if (!string.IsNullOrEmpty(SiteHelper.GetAppsetString("NearParkingKM")))
                km = double.Parse(SiteHelper.GetAppsetString("NearParkingKM"));
            if (!string.IsNullOrEmpty(SiteHelper.GetAppsetString("PageNum")))
                pageNum = int.Parse(SiteHelper.GetAppsetString("PageNum"));

            if (res["Longitude"] == null || res["Longitude"].ToString().Trim().Length <= 0 ||
                res["Latitude"] == null || res["Latitude"].ToString().Trim().Length <= 0 ||
                res["PageNum"] == null || res["PageNum"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"] == null || res["CurrentPage"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                //pageNum = int.Parse(res["PageNum"].ToString().Trim());
                currentPage = int.Parse(res["CurrentPage"].ToString().Trim());
                string longitude = res["Longitude"].ToString().Trim();
                string latitude = res["Latitude"].ToString().Trim();

                ListApiResp resp = new ListApiResp();
                resp.Code = "-1";
                resp.Message = "";

                int count = 0;
                ServiceAreaManager areaManager = new ServiceAreaManager();
                DataTable dt= areaManager.GetNearParking(km, longitude, latitude, currentPage, pageNum,ref count);
                if(dt!=null && dt.Rows.Count>0)
                {
                    List<GetNearParkingResponseDataItem> list = new List<GetNearParkingResponseDataItem>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        GetNearParkingResponseDataItem item = new GetNearParkingResponseDataItem();
                        item.name = dr["ThisName"].ToString();
                        item.center = dr["Longitude"].ToString() + "," + dr["Latitude"].ToString();
                        item.points = dr["Coordinates"].ToString();
                        list.Add(item);
                    }
                    resp.List = list;
                    resp.Total = count;
                    resp.Code = "0";
                }

                return JsonConvert.SerializeObject(resp);
            }
        }
        
    }

    /// <summary>
    /// 接口GetNearParkingResponse返回数据列表项
    /// </summary>
    class GetNearParkingResponseDataItem
    {
        public string name;

        public string center;

        public string points;
    }
}