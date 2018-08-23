using Asiasofti.SmartVehicle.Common;
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
    /// 车辆基础数据
    /// </summary>
    public class GetCars:IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();

            List<GetCars_Item> data = new List<GetCars_Item>();

            SysSettingManager settingManager=new SysSettingManager();
            DictManager dictManager = new DictManager();
            ServiceAreaManager areaManager = new ServiceAreaManager();
            string depositMoney = settingManager.GetValueByKey("GoldDepositAmount");

            DataTable city_dt = dictManager.GetDictList("03");
            if(city_dt!=null && city_dt.Rows.Count>0)
            {
                foreach(DataRow city_dr in city_dt.Rows)
                {
                    GetCars_Item item = new GetCars_Item();
                    Hashtable area_ht = areaManager.GetServiceAreaByCityCode(city_dr["remark"].ToString());
                    if (area_ht != null && area_ht.Keys.Count > 0)
                    {
                        item.coordinatePoints = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates").Replace(";", "|");
                        item.coordinate_center = SiteHelper.GetHashTableValueByKey(area_ht, "Longitude") + "," + SiteHelper.GetHashTableValueByKey(area_ht, "Latitude");
                    }
                    item.city_name = city_dr["name"].ToString();

                    DataTable car_dt = areaManager.GetVehiclesByCity(item.city_name, 1, 9999);
                    if (car_dt != null && car_dt.Rows.Count>0)
                    {
                        List<GetCars_Car> car_list = new List<GetCars_Car>();
                        foreach (DataRow dr in car_dt.Rows)
                        {
                            GetCars_Car car = new GetCars_Car();
                            car.id = dr["ID"].ToString();
                            car.plate_no = dr["LicenseNumber"].ToString();
                            car.carname = dr["Brand"].ToString();
                            car.start_price = double.Parse(dr["MinPrice"].ToString());
                            car.minute_price = double.Parse(dr["MinutePrice"].ToString());
                            car.mile_price = double.Parse(dr["KmPrice"].ToString());
                            car.deposit_money = double.Parse(depositMoney);

                            car_list.Add(car);
                        }
                        item.cars = car_list;
                    }
                    if(item.cars!=null && item.cars.Count>0)
                        data.Add(item);
                }
            }

            if (data.Count > 0)
            {
                resp.code = "0";
                resp.msg = "成功";
                resp.data = data;
            }
            else
            {
                resp.code = "-1";
                resp.msg = "未找到车辆信息";
            }

            return resp;
        }
    }

    public class GetCars_Item
    {
        public string city_name;

        public string coordinatePoints;

        public string coordinate_center;

        public List<GetCars_Car> cars;
    }

    public class GetCars_Car
    {
        public string id;

        public string plate_no;

        public string carname;

        public double start_price;

        public double minute_price;

        public double mile_price;

        public double deposit_money;
    }
}