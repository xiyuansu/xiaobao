using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace YR.Web.api.baojia.service
{
    /// <summary>
    /// 运营网点基础数据
    /// </summary>
    public class GetStations:IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();

            List<GetStations_Item> data = new List<GetStations_Item>();

            SysSettingManager settingManager = new SysSettingManager();
            ServiceAreaManager areaManager = new ServiceAreaManager();
            string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
            DataTable dt= areaManager.GetAllParkings(1, 99999);
            if (returnVehicleMode == "1" && dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GetStations_Item item = new GetStations_Item();
                    item.stationId = dr["ID"].ToString();
                    item.stationName = dr["ThisName"].ToString();
                    item.stationLat = double.Parse(dr["Latitude"].ToString());
                    item.stationLon = double.Parse(dr["Longitude"].ToString());
                    item.stationAddress = dr["Address"].ToString();
                    item.startTime = 0;
                    item.endTime = 2400;
                    item.radius = 0.00;
                    item.parkType = 2;
                    item.coordinatepoints = dr["Coordinates"].ToString().Replace(";","|");
                    data.Add(item);
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

            return resp;
        }
    }

    public class GetStations_Item
    {
        public string stationId;

        public string stationName;

        public double stationLat;

        public double stationLon;

        public string stationAddress;

        public int startTime;

        public int endTime;

        public double radius;

        public int parkType;

        public string coordinatepoints;
    }

}