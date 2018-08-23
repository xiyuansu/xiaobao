using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 获取车辆车辆轨迹数据
    /// </summary>
    public class GetVehicleTrace : IApiAction2
    {
        private string uid = string.Empty;
        private string vehicleid = string.Empty;
        private DateTime date;
        private DateTime? date2 = null;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["VehicleID"] == null || res["VehicleID"].ToString().Trim().Length <= 0 ||
                res["Date"] == null || res["Date"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    vehicleid = res["VehicleID"].ToString().Trim();
                    date = DateTime.Parse(res["Date"].ToString().Trim());
                    if (res["Date2"] != null && res["Date2"].ToString().Trim().Length > 0)
                        date2 = DateTime.Parse(res["Date2"].ToString().Trim());
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }

                VehicleManager vm = new VehicleManager();
                DateTime begin = date;
                DateTime end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                if (date2.HasValue)
                    end = date2.Value;
                DataTable traces = vm.GetVehicleTrace(vehicleid, null, begin, end);
                if (traces == null || traces.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "亲，未找到车辆轨迹数据");
                }
                else
                {
                    traces.Columns.Remove("ID");
                    traces.Columns.Remove("VehicleID");
                    traces.Columns.Remove("OrderID");
                    //traces.Columns.Remove("Latitude");
                    //traces.Columns.Remove("Longitude");
                    //traces.Columns.Remove("BD_latitude");
                    //traces.Columns.Remove("BD_longitude");
                    //traces.Columns.Remove("AMAP_latitude");
                    //traces.Columns.Remove("AMAP_longitude");
                    traces.Columns.Remove("GPSNum");
                    traces.Columns.Remove("Direction");
                    traces.Columns.Remove("Speed");
                    traces.Columns.Remove("Power");
                    traces.Columns.Remove("LockState");
                    traces.Columns.Remove("LightState");
                    traces.Columns.Remove("RecordTime");
                    return DataTableToJson("success", "获取车辆轨迹数据成功", traces, "VehicleTrace");
                }

            }
        }

        public static string DataTableToJson(string state, string message, DataTable dt, string dtName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"state\":").Append("\"" + state + "\",\"message\":\"" + message + "\",\"content\":");
            sb.Append("{\"");
            sb.Append(dtName);
            sb.Append("\":[");
            if (DataTableHelper.IsExistRows(dt))
            {
                double lastLat = 0.00;
                double lastLng = 0.00;
                foreach (DataRow dr in dt.Rows)
                {

                    double lat = double.Parse(dr["Latitude"].ToString());
                    double lng = double.Parse(dr["Longitude"].ToString());
                    if (lastLat == lat && lastLng == lng)
                    {
                        continue;
                    }
                    lastLat = lat;
                    lastLng = lng;

                    sb.Append("{");
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        sb.Append("\"");
                        sb.Append(dc.ColumnName);
                        sb.Append("\":\"");
                        if (dr[dc] != null && dr[dc] != DBNull.Value && dr[dc].ToString() != "")
                            sb.Append(dr[dc].ToString().Trim()).Replace("\\", "/");
                        else
                            sb.Append("");
                        sb.Append("\",");
                    }

                    sb = sb.Remove(sb.Length - 1, 1);
                    sb.Append("},");
                }
                sb = sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]}}");
            return sb.ToString();
        }

    }
}