using Asiasofti.SmartVehicle.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YR.Common.DotNetBean;
using YR.Common.DotNetCode;
using YR.Common.DotNetJson;
using YR.Common.ZCloudUtil;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 一动平台控制器
    /// </summary>
    public class YDVehicleController : IVehicleController
    {
        /// <summary>
        /// 开车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool Open(string vehicleid)
        {
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht= vm.GetVehicleInfoByID(vehicleid);

                Dictionary<string,string> dict=new Dictionary<string,string>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("cmd", "carControl");
                dict.Add("type ","4");
                dict.Add("directRt","true");

                string sign = ZCloudSignUtil.getSign(dict, YDCloudConfig.md5_key);
                dict.Add("sign",sign);

                //string postData=ZCloudCore.createLinkString(dict);
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = HttpUtil.Post(YDCloudConfig.api_url, postData, "utf-8");
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                    result = true;

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 锁车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool Close(string vehicleid)
        {
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("cmd", "carControl");
                dict.Add("type ", "3");
                dict.Add("directRt", "true");

                string sign = ZCloudSignUtil.getSign(dict, YDCloudConfig.md5_key);
                dict.Add("sign", sign);

                //string postData = ZCloudCore.createLinkString(dict);
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = HttpUtil.Post(YDCloudConfig.api_url, postData, "utf-8");
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                    result = true;

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 寻车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool Find(string vehicleid)
        {
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("cmd", "carControl");
                dict.Add("type ", "6");

                string sign = ZCloudSignUtil.getSign(dict, YDCloudConfig.md5_key);
                dict.Add("sign", sign);

                //string postData = ZCloudCore.createLinkString(dict);
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = HttpUtil.Post(YDCloudConfig.api_url, postData, "utf-8");
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                    result = true;

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 开座垫指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool OpenSeat(string vehicleid)
        {
            return false;
        }

        /// <summary>
        /// 车辆断电指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool PowerOff(string vehicleid)
        {
            return false;
        }

        /// <summary>
        /// 开电池锁指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool OpenBatteryLock(string vehicleid)
        {
            return false;
        }

        /// <summary>
        /// 关电池锁指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool CloseBatteryLock(string vehicleid)
        {
            return false;
        }
    }
}
