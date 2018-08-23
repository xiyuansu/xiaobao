using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Configuration;
using YR.Web.api.api_class;
using Newtonsoft.Json;
using Asiasofti.SmartVehicle.Manager;
using System.Collections;
using Asiasofti.SmartVehicle.Common;
using YR.Common.DotNetJson;
using System.Data;
using Asiasofti.SmartVehicle.Common.Enum;
using Newtonsoft.Json.Converters;
using YR.Common.SystemInfo;

namespace YR.Web.api.app
{
    /// <summary>
    /// VehicleController 的摘要说明
    /// </summary>
    public class VehicleController : IHttpHandler, IRequiresSessionState
    {
        private Hashtable hashtableResult = null;

        public Hashtable HashtableResult
        {
            get
            {
                return this.hashtableResult;
            }
            set
            {
                this.hashtableResult = value;
            }
        }

        private bool CheckAuth(HttpContext context)
        {
            CompilationSection ds = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation");
            if (ds.Debug)
            {
                return true;
            }

            bool result = false;
            string access_token = context.Request["access_token"];
            List<ApiToken> tokenList = context.Application["api_token"] as List<ApiToken>;
            if (tokenList == null)
                tokenList = new List<ApiToken>();
            ApiToken token = (from i in tokenList where i.access_token == access_token select i).SingleOrDefault();

            if (token != null)
            {
                if (token.update_time.AddSeconds(7200) <= DateTime.Now)
                {
                    result = true;
                }
            }
            return result;
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.CacheControl = "no-cache";
                context.Response.AddHeader("Pragma", "no-cache");
                context.Response.ContentType = "text/plain";

                string method = context.Request["method"];
                if (string.IsNullOrEmpty(method))
                    throw new Exception("系统参数错误");

                //if (!CheckAuth(context))
                //    throw new Exception("无权访问此操作");

                switch (method)
                {
                    case "GetVehicles":
                        GetVehicles(context);
                        break; 
                    case "GetCupInfo":
                        GetCupInfo(context);
                        break;
                    case "GetNearVehicles":
                        GetNearVehicles(context);
                        break;
                    case "GetVehicleByID":
                        GetVehicleByID(context);
                        break;
                    case "GetVehicleTrace":
                        GetVehicleTrace(context);
                        break;
                    case "GetOrdersByVehicle":
                        GetOrdersByVehicle(context);
                        break;
                    case "GetServiceArea":
                        GetServiceArea(context);
                        break;
                    case "GetServiceAreaByCityID":
                        GetServiceAreaByCityID(context);
                        break;
                    case "GetServiceAreaByID":
                        GetServiceAreaByID(context);
                        break;
                    case "GetParkingList":
                        GetParkingList(context);
                        break;
                    case "GetForbidList":
                        GetForbidList(context);
                        break;
                    default:
                        ApiResp resp = new ApiResp();
                        resp.Code = "-1";
                        resp.Message = "调用异常,无效的方法调用";
                        context.Response.Write(JsonConvert.SerializeObject(resp)); ;
                        break;
                }
            }
            catch (Exception e)
            {
                ApiResp resp = new ApiResp();
                resp.Code = "-1";
                resp.Message = "系统异常," + e.Message;
                context.Response.Write(JsonConvert.SerializeObject(resp));
            }
        }

        private void GetCupInfo(HttpContext context)
        {
            SystemInfo sys = new SystemInfo();
            const int GB_DIV = 1024 * 1024 * 1024;
            double cpu = Math.Round(sys.CpuLoad, 2);
            decimal memory = Math.Round((decimal)(sys.PhysicalMemory - sys.MemoryAvailable) / sys.PhysicalMemory, 2) * 100;
            string useInfo = string.Format("系统CPU使用率：{0}%，系统内存使用大小：{1}GB，{2}%", Math.Round(sys.CpuLoad, 2), Math.Round((sys.PhysicalMemory - sys.MemoryAvailable) / (double)GB_DIV, 2),memory);
            ApiResp resp = new ApiResp();
            resp.Code = "0";
            resp.Message = useInfo;
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// 获取所有车辆信息
        /// </summary>
        /// <param name="context"></param>
        private void GetVehicles(HttpContext context)
        {
            string name = context.Request["name"];
            int? vstate = null;
            if (!string.IsNullOrEmpty(context.Request["vstate"]))
            {
                int val = 0;
                int.TryParse(context.Request["vstate"], out val);
                vstate = val;
            }
            int page_index = 0;
            int.TryParse(context.Request["page_index"], out page_index);
            int page_size = 0;
            int.TryParse(context.Request["page_size"], out page_size);
            if (page_size == 0)
                page_size = 10000;

            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "0";
            resp.Message = "";
            resp.Data = "";
            VehicleManager vm = new VehicleManager();

            DataTable vehicles = vm.GetVehicles(name, vstate, page_index + 1, page_size);
            if (vehicles != null && vehicles.Rows.Count > 0)
            {
                foreach (DataRow dr in vehicles.Rows)
                {
                    dr["ThumbImageUrl"] = SiteHelper.GetFileServer() + dr["ThumbImageUrl"].ToString();
                    decimal electricity = 0;
                    decimal.TryParse(dr["Electricity"].ToString(), out electricity);
                    int dianliang = Decimal.ToInt32(electricity);
                    dr["Electricity"] = dianliang + "%";
                    if (dr["LastUpdateTime"] != null && dr["LastUpdateTime"] != DBNull.Value)
                        dr["LastUpdateTime"] = DateTime.Parse(dr["LastUpdateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

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
                Newtonsoft.Json.Converters.IsoDateTimeConverter timeFormat = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                resp.Data = Newtonsoft.Json.JsonConvert.SerializeObject(vehicles, Formatting.Indented, timeFormat);
            }
            else
            {
                resp.Code = "-1";
                resp.Message = "未找到相关车辆";
            }

            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// 获取附近车辆详细信息
        /// </summary>
        /// <param name="context"></param>
        private void GetVehicleByID(HttpContext context)
        {
            string vehicleID = string.Empty;
            string weidu = string.Empty;
            string jingdu = string.Empty;

            if (context.Request["VehicleID"] == null ||
                context.Request["Latitude"] == null ||
                context.Request["Longitude"] == null ||
            context.Request["VehicleID"].ToString().Trim().Length <= 0 ||
               context.Request["Latitude"].ToString().Trim().Length <= 0 ||
               context.Request["Longitude"].ToString().Trim().Length <= 0)
            {
                context.Response.Write(SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整"));
                context.Response.End();
            }
            else
            {
                vehicleID = context.Request["VehicleID"].ToString().Trim();
                weidu = context.Request["Latitude"].ToString().Trim();
                jingdu = context.Request["Longitude"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                SystemImagesManager sim = new SystemImagesManager();
                Hashtable vehicle = vm.GetVehicleInfoByID(vehicleID, weidu, jingdu);
                string thumbImg = string.Empty;
                string detailsImg = string.Empty;
                if (vehicle == null || vehicle.Keys.Count <= 0)
                {
                    context.Response.Write(SiteHelper.GetJsonFromHashTable(null, "faild", "车辆数据为空"));
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
                        /*LatLng latlng = SiteHelper.GetBaiduByGPS(lat, lng);
                        vehicle["LATITUDE"] = latlng.latitude;
                        vehicle["LONGITUDE"] = latlng.longitude;
                        if (vehicle["ADDRESS"].ToString().Length <= 0)
                        {
                            vehicle["ADDRESS"] = SiteHelper.GetLocationByGPS(vehicle["LONGITUDE"].ToString(), vehicle["LATITUDE"].ToString());
                        }*/
                        LatLng latlng=new LatLng(double.Parse(lat),double.Parse(lng));
                        latlng = SiteHelper.TransformFromWGSToGCJ(latlng);
                        vehicle["LATITUDE"]=latlng.latitude;
                        vehicle["LONGITUDE"] = latlng.longitude;
                    }
                    

                    decimal electricity = 0;
                    decimal.TryParse(vehicle["ELECTRICITY"].ToString(), out electricity);
                    int dianliang = Decimal.ToInt32(electricity);
                    vehicle["ELECTRICITY"] = dianliang + "%";

                    vehicle["PRICESYSTEM"] = SiteHelper.GetFileServer() + "/AppPage/BusinessInfo.aspx?id=" + SiteHelper.GetHashTableValueByKey(vehicle, "ID") + "&type=" + BusinessType.Vehicle.GetHashCode() + "&field=" + BusinessFieldType.PRICESYSTEM.GetHashCode();
                    vehicle["VEHICLEREMARK"] = SiteHelper.GetFileServer() + "/AppPage/BusinessInfo.aspx?id=" + SiteHelper.GetHashTableValueByKey(vehicle, "ID") + "&type=" + BusinessType.Vehicle.GetHashCode() + "&field=" + BusinessFieldType.VEHICLEREMARK.GetHashCode();
                    vehicle["TRANSPORTINFORMATION"] = SiteHelper.GetFileServer() + "/AppPage/BusinessInfo.aspx?id=" + SiteHelper.GetHashTableValueByKey(vehicle, "ID") + "&type=" + BusinessType.Vehicle.GetHashCode() + "&field=" + BusinessFieldType.TRANSPORTINFORMATION.GetHashCode();
                    context.Response.Write(SiteHelper.GetJsonFromHashTable(vehicle, "success", "获取数据成功", "VehicleInfo"));
                }
            }
        }

        /// <summary>
        /// 获取附近车辆信息
        /// </summary>
        /// <param name="context"></param>
        private void GetNearVehicles(HttpContext context)
        {
            double km = 100;
            int pageNum = 100;
            int currentPage = 1;
            string weidu = string.Empty;
            string jingdu = string.Empty;
            if (!string.IsNullOrEmpty(SiteHelper.GetAppsetString("NearVehicleKM")))
                km = double.Parse(SiteHelper.GetAppsetString("NearVehicleKM"));
            if (!string.IsNullOrEmpty(SiteHelper.GetAppsetString("PageNum")))
                pageNum = int.Parse(SiteHelper.GetAppsetString("PageNum"));
            if (context.Request["Latitude"] == null ||
                context.Request["Longitude"] == null ||
                context.Request["PageNum"] == null ||
                context.Request["CurrentPage"] == null ||
                context.Request["Latitude"].ToString().Trim().Length <= 0 ||
                context.Request["Longitude"].ToString().Trim().Length <= 0 ||
                context.Request["PageNum"].ToString().Trim().Length <= 0 ||
                context.Request["CurrentPage"].ToString().Trim().Length <= 0)
            {
                context.Response.Write(SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整"));
            }
            else
            {
                try
                {
                    //pageNum = int.Parse(res["PageNum"].ToString().Trim());
                    currentPage = int.Parse(context.Request["CurrentPage"].ToString().Trim());
                    weidu = context.Request["Latitude"].ToString().Trim();
                    jingdu = context.Request["Longitude"].ToString().Trim();
                }
                catch
                {
                    context.Response.Write(SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确"));
                    context.Response.End();
                }
                VehicleManager vm = new VehicleManager();
                DataTable vehicles = vm.GetNearVehicles(km, weidu, jingdu, currentPage, pageNum);
                if (vehicles == null || vehicles.Rows.Count <= 0)
                {
                    context.Response.Write(SiteHelper.GetJsonFromHashTable(null, "faild", "亲，没有数据了"));
                    context.Response.End();
                }
                else
                {
                    foreach (DataRow dr in vehicles.Rows)
                    {
                        dr["ThumbImageUrl"] = SiteHelper.GetFileServer() + dr["ThumbImageUrl"].ToString();
                        decimal electricity = 0;
                        decimal.TryParse(dr["Electricity"].ToString(), out electricity);
                        int dianliang = Decimal.ToInt32(electricity);
                        dr["Electricity"] = dianliang + "%";

                        string lng = dr["Longitude"].ToString();
                        string lat = dr["Latitude"].ToString();
                        if (!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat))
                        {
                            LatLng latlng = SiteHelper.GetBaiduByGPS(lat, lng);
                            if (latlng != null)
                            {
                                dr["Longitude"] = latlng.longitude;
                                dr["Latitude"] = latlng.latitude;
                                if (dr["Address"].ToString().Trim().Length <= 0)
                                {
                                    dr["Address"] = SiteHelper.GetLocationByGPS(dr["Latitude"].ToString(), dr["Longitude"].ToString());
                                }
                            }
                        }
                    }
                    context.Response.Write(JsonHelper.DataTableToJson("success", "获取数据成功", vehicles, "VehicleInfo"));
                }
            }
        }

        private void GetVehicleTrace(HttpContext context)
        {
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "0";
            resp.Message = "";

            string vehicleid = "";
            string orderid = "";
            DateTime? begin = null;
            DateTime? end = null;
            vehicleid = context.Request["VehicleID"];
            if (!string.IsNullOrEmpty(context.Request["OrderID"]))
                orderid = context.Request["OrderID"].ToString().Trim();
            if (string.IsNullOrEmpty(orderid))
            {
                if (!string.IsNullOrEmpty(context.Request["begin"]))
                    begin = DateTime.Parse(context.Request["begin"].ToString().Trim());
                else
                    begin = DateTime.Now.Date.AddDays(-1);
                if (!string.IsNullOrEmpty(context.Request["end"]))
                    end = DateTime.Parse(context.Request["end"].ToString().Trim());
                else
                    end = DateTime.Now;
            }
            else
            {
                begin = null;
                end = null;
            }
            
            VehicleManager vm = new VehicleManager();
            DataTable traces = vm.GetVehicleTrace(vehicleid, orderid, begin, end);
            if (traces != null && traces.Rows.Count > 0)
            {
                traces.Columns.Remove("ID");
                traces.Columns.Remove("VehicleID");
                traces.Columns.Remove("OrderID");
                traces.Columns.Remove("Latitude");
                traces.Columns.Remove("Longitude");
                traces.Columns.Remove("BD_latitude");
                traces.Columns.Remove("BD_longitude");
                //traces.Columns.Remove("AMAP_latitude");
                //traces.Columns.Remove("AMAP_longitude");
                traces.Columns.Remove("GPSNum");
                traces.Columns.Remove("Direction");
                traces.Columns.Remove("Speed");
                traces.Columns.Remove("Power");
                traces.Columns.Remove("LockState");
                traces.Columns.Remove("LightState");
                traces.Columns.Remove("RecordTime");
                resp.Data = traces;
            }
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        private void GetOrdersByVehicle(HttpContext context)
        {
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "0";
            resp.Message = "";

            string vehicleid = "";
            DateTime? begin = null;
            DateTime? end = null;
            vehicleid = context.Request["VehicleID"].ToString().Trim();
            if (context.Request["begin"] != null)
                begin = DateTime.Parse(context.Request["begin"].ToString().Trim());
            if (context.Request["end"] != null)
                end = DateTime.Parse(context.Request["end"].ToString().Trim());
            int page_index = 0;
            int.TryParse(context.Request["page_index"], out page_index);
            int page_size = 0;
            int.TryParse(context.Request["page_size"], out page_size);
            if (page_size == 0)
                page_size = 100;

            OrdersManager om = new OrdersManager();
            DataTable orders = om.GetOrdersInfoByVehicle(vehicleid, begin, end, page_index + 1, page_size);
            if (orders != null && orders.Rows.Count > 0)
            {
                resp.Data = orders;
            }
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            context.Response.Write(JsonConvert.SerializeObject(resp, timeFormat));
        }

        /// <summary>
        /// 根据城市获取运营区域
        /// </summary>
        /// <param name="context"></param>
        private void GetServiceArea(HttpContext context)
        {
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "-1";
            resp.Message = "";

            string citycode = "";
            citycode = context.Request["city"].ToString().Trim();

            ServiceAreaManager areaManager = new ServiceAreaManager();
            DataTable dt = areaManager.GetServiceAreaList(citycode);// areaManager.GetServiceAreaListByCityCode(citycode);
            if (dt != null)
            {
                resp.Data = dt;
                resp.Code = "0";
            }
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// 根据城市获取运营区域
        /// </summary>
        /// <param name="context"></param>
        private void GetServiceAreaByCityID(HttpContext context)
        {
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "-1";
            resp.Message = "";

            string cityID= "";
            cityID = context.Request["cityID"].ToString().Trim();

            ServiceAreaManager areaManager = new ServiceAreaManager();
            DataTable dt = areaManager.GetServiceAreaListByCityID(cityID);
            if (dt != null)
            {
                resp.Data = dt;
                resp.Code = "0";
            }
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// 根据城市获取运营区域
        /// </summary>
        /// <param name="context"></param>
        private void GetServiceAreaByID(HttpContext context)
        {
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "-1";
            resp.Message = "";

            string vehicleID= context.Request["vehicleID"].ToString().Trim();

            ServiceAreaManager areaManager = new ServiceAreaManager();
            DataTable dt = areaManager.GetServiceAreaListByID(vehicleID);
            if (dt != null)
            {
                resp.Data = dt;
                resp.Code = "0";
            }
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// 根据城市获取停车网点
        /// </summary>
        /// <param name="context"></param>
        private void GetParkingList(HttpContext context)
        {
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "-1";
            resp.Message = "";

            string citycode = "";
            citycode = context.Request["city"].ToString().Trim();

            SysSettingManager settingManager = new SysSettingManager();
            string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
            if (returnVehicleMode == "1")
            {
                ServiceAreaManager areaManager = new ServiceAreaManager();
                DataTable dt = areaManager.GetParkingListByCityCode(citycode);
                if (dt != null)
                {
                    resp.Data = dt;
                    resp.Code = "0";
                }
            }
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// 根据城市获取停车网点
        /// </summary>
        /// <param name="context"></param>
        private void GetForbidList(HttpContext context)
        {
            ObjectApiResp resp = new ObjectApiResp();
            resp.Code = "-1";
            resp.Message = "";

            string citycode = "";
            citycode = context.Request["city"].ToString().Trim();

            SysSettingManager settingManager = new SysSettingManager();
            string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
            if (returnVehicleMode == "1")
            {
                ServiceAreaManager areaManager = new ServiceAreaManager();
                DataTable dt = areaManager.GetForbidListByCityCode(citycode);
                if (dt != null)
                {
                    resp.Data = dt;
                    resp.Code = "0";
                }
            }
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}