using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System.Data;
using YR.Common.DotNetJson;
using YR.Web.api.api_class;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取指定马厩内的车辆
    /// </summary>
    public partial class GetVehiclesByParkingID : System.Web.UI.Page
    {
        private string parkingID = null;
        private int pageNum =100;
        private int currentPage = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageNum"]))
                pageNum = int.Parse(ConfigurationManager.AppSettings["PageNum"].ToString());
            RequestControler rc = (RequestControler)HttpContext.Current.Handler;
            Hashtable res = rc.HashtableResult;
            if (res["ParkingID"] == null ||
                res["PageNum"] == null ||
                res["CurrentPage"] == null ||
                res["ParkingID"].ToString().Trim().Length <= 0 ||
                res["PageNum"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"].ToString().Trim().Length <= 0)
            {
                Response.Write(SiteHelper.Encrypt(SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整"), rc.EncryptionKey));
                Response.End();
            }
            else
            {
                try
                {
                    //pageNum = int.Parse(res["PageNum"].ToString().Trim());
                    currentPage = int.Parse(res["CurrentPage"].ToString().Trim());
                    parkingID = res["ParkingID"].ToString().Trim();
                }
                catch
                {
                    Response.Write(SiteHelper.Encrypt(SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确"), rc.EncryptionKey));
                    Response.End();
                }
                VehicleParkingManager pm = new VehicleParkingManager();
                DataTable vehicles = pm.GetVehiclesByParkingId(parkingID, currentPage, pageNum);
                if (vehicles == null || vehicles.Rows.Count <= 0)
                {
                    Response.Write(SiteHelper.Encrypt(SiteHelper.GetJsonFromHashTable(null, "faild", "亲，没有数据了"), rc.EncryptionKey));
                    Response.End();
                }
                else
                {
                    foreach (DataRow dr in vehicles.Rows)
                    {
                        dr["ThumbImageUrl"] = SiteHelper.GetImageDomainServer() + dr["ThumbImageUrl"].ToString();
                        decimal electricity = 0;
                        decimal.TryParse(dr["Electricity"].ToString(), out electricity);
                        int dianliang = Decimal.ToInt32(electricity);
                        dr["Electricity"] = dianliang + "%";

                        string lng = dr["Longitude"].ToString();
                        string lat = dr["Latitude"].ToString();
                        if (!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat))
                        {
                            string latlng = GetBaiduCord(double.Parse(lat), double.Parse(lng), "1", "5");
                            if (!string.IsNullOrEmpty(latlng))
                            {
                                dr["Longitude"] = latlng.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[1];
                                dr["Latitude"] = latlng.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];

                                if (res["Client"] != null && "iOS" == res["Client"].ToString())
                                {
                                    LatLng ll = new LatLng(double.Parse(lng), double.Parse(lat));
                                    ll = SiteHelper.TransformFromWGSToGCJ(ll);
                                    dr["Longitude"] = ll.latitude;
                                    dr["Latitude"] = ll.longitude;
                                }
                                else
                                {
                                    if (dr["Address"].ToString().Trim().Length <= 0)
                                    {
                                        dr["Address"] = SiteHelper.GetLocationByGPS(dr["Latitude"].ToString(), dr["Longitude"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    Response.Write(SiteHelper.Encrypt(JsonHelper.DataTableToJson("success", "获取数据成功", vehicles, "VehicleInfo"), rc.EncryptionKey));
                    Response.End();
                }
            }
        }

        public string GetBaiduCord(double lat, double lng, string from, string to)
        {
            string latlng = "";
            try
            {
                ApiKit apiKit = new ApiKit("http://api.map.baidu.com/geoconv/v1/");
                Dictionary<string, string> parames = new Dictionary<string, string>();
                parames.Add("coords", string.Format("{0},{1}", lat, lng));
                parames.Add("from", from);
                parames.Add("to", to);
                parames.Add("output", "json");
                parames.Add("ak", apiKit.AppKey);
                string retstr = apiKit.get(parames);
                JObject o = JObject.Parse(retstr);
                string status = o["status"].ToString();
                if (status.CompareTo("0") == 0)
                {
                    latlng += string.Format("{0}", o["result"][0]["x"]);
                    latlng += string.Format(",{0}", o["result"][0]["y"]);
                }
                return latlng;
            }
            catch
            {
                return latlng;
            }
        }
        
    }
}