using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
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
    /// 车辆实时信息
    /// </summary>
    public class GetNowCars : IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();

            string plate_no = null;
            if (params_ht["plate_no"] != null)
            {
                plate_no = params_ht["plate_no"].ToString();
                if (plate_no != null && (plate_no.StartsWith("http://") || plate_no.StartsWith("https://")))
                {
                    //http://api.qingniaokeji.cn/download/app.aspx?no=xxxxxxxxxxxxx
                    Uri uri = new Uri(plate_no);
                    System.Collections.Specialized.NameValueCollection coll = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    plate_no = coll.Get("no");
                }
            }

            List<GetNowCars_Item> data = new List<GetNowCars_Item>();

            VehicleManager vm = new VehicleManager();
            ServiceAreaManager areaManager = new ServiceAreaManager();
            if (string.IsNullOrEmpty(plate_no))
            {
                DataTable dt = areaManager.GetVehiclesByCity(null, 1, 9999);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        GetNowCars_Item item = new GetNowCars_Item();
                        item.id = dr["ID"].ToString();
                        item.plate_no = dr["LicenseNumber"].ToString();
                        item.lng = double.Parse(dr["Longitude"].ToString());
                        item.lat = double.Parse(dr["Latitude"].ToString());
                        if (dr["LastUpdateTime"].ToString().Trim().Length > 0)
                            item.datetime = (long)(DateTime.Parse(dr["LastUpdateTime"].ToString()) - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
                        item.mileage = dr["ExpectRange"].ToString();
                        item.power= (int)double.Parse(dr["Electricity"].ToString());
                        if (item.power > 100)
                            item.power = 100;
                        if (item.power < 0)
                            item.power = 0;
                        item.minute_price = double.Parse(dr["MinutePrice"].ToString());
                        item.mile_price = double.Parse(dr["KmPrice"].ToString());
                        item.start_price = double.Parse(dr["MinPrice"].ToString());
                        item.other_price = "每日封顶价格：" + dr["MaxPrice"].ToString() + "元";

                        string use_state = dr["UseState"].ToString();
                        string vehicle_state = dr["VehicleState"].ToString();
                        double electricity = 0.00;
                        double.TryParse(dr["Electricity"].ToString(), out electricity);
                        DateTime last_update_time = DateTime.Now;
                        if (dr["LastUpdateTime"].ToString().Trim().Length > 0)
                            DateTime.TryParse(dr["LastUpdateTime"].ToString(), out last_update_time);

                        item.is_rent = 0;
                        if (vehicle_state == VehicleState.Use.GetHashCode().ToString() && use_state == VehicleUseState.Free.GetHashCode().ToString() && electricity>20 && (DateTime.Now - last_update_time).TotalMinutes < 5)
                            item.is_rent = 1;
                        data.Add(item);
                    }
                    resp.code = "0";
                    resp.msg = "成功";
                    resp.data = data;
                }
                else
                {
                    resp.code = "-1";
                    resp.msg = "未找到车辆";
                }
            }
            else
            {
                Hashtable ht = vm.GetVehicleInfoByIDORNumber(plate_no);
                if (ht != null && ht.Keys.Count > 0)
                {
                    GetNowCars_Item item = new GetNowCars_Item();
                    item.id = SiteHelper.GetHashTableValueByKey(ht, "ID");
                    item.plate_no = SiteHelper.GetHashTableValueByKey(ht, "LicenseNumber");
                    item.lng = double.Parse(SiteHelper.GetHashTableValueByKey(ht, "Longitude"));
                    item.lat = double.Parse(SiteHelper.GetHashTableValueByKey(ht, "Latitude"));
                    if (SiteHelper.GetHashTableValueByKey(ht, "LastUpdateTime").Length > 0)
                        item.datetime = (long)(DateTime.Parse(SiteHelper.GetHashTableValueByKey(ht, "LastUpdateTime")) - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
                    item.mileage = SiteHelper.GetHashTableValueByKey(ht, "ExpectRange");
                    item.power = (int)double.Parse(SiteHelper.GetHashTableValueByKey(ht, "Electricity"));
                    if (item.power > 100)
                        item.power = 100;
                    if (item.power < 0)
                        item.power = 0;
                    item.minute_price = double.Parse(SiteHelper.GetHashTableValueByKey(ht, "MinutePrice"));
                    item.mile_price = double.Parse(SiteHelper.GetHashTableValueByKey(ht, "KmPrice"));
                    item.start_price = double.Parse(SiteHelper.GetHashTableValueByKey(ht, "MinPrice"));
                    item.other_price = "每日封顶价格：" + SiteHelper.GetHashTableValueByKey(ht, "MaxPrice") + "元";

                    string use_state=SiteHelper.GetHashTableValueByKey(ht, "UseState");
                    string vehicle_state=SiteHelper.GetHashTableValueByKey(ht, "VehicleState");
                    double electricity = 0.00;
                    double.TryParse(SiteHelper.GetHashTableValueByKey(ht, "Electricity"), out electricity);
                    DateTime last_update_time = DateTime.Now;
                    if (SiteHelper.GetHashTableValueByKey(ht, "LastUpdateTime").Length>0)
                        DateTime.TryParse(SiteHelper.GetHashTableValueByKey(ht, "LastUpdateTime"),out last_update_time);
                    item.is_rent = 0;
                    if (vehicle_state == VehicleState.Use.GetHashCode().ToString() && use_state == VehicleUseState.Free.GetHashCode().ToString() && electricity>20 && (DateTime.Now - last_update_time).Minutes < 5)
                        item.is_rent = 1;

                    data.Add(item);

                    resp.code = "0";
                    resp.msg = "成功";
                    resp.data = data;
                }
                else
                {
                    resp.code = "-1";
                    resp.msg = "未找到车辆";
                }
            }

            return resp;
        }
    }

    public class GetNowCars_Item
    {
        public string id;

        public string plate_no;

        public double lng;

        public double lat;

        public string mileage;

        public int power;

        public long datetime;

        public double start_price;

        public double minute_price;

        public double mile_price;

        public string other_price;

        public int is_rent;
    }

}