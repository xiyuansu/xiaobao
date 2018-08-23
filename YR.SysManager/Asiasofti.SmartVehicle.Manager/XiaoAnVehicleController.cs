using Asiasofti.SmartVehicle.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Common.ZCloudUtil;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 小安盒子控制器
    /// </summary>
    public class XiaoAnVehicleController : IVehicleController
    {
        private static Log Logger = LogFactory.GetLogger(typeof(XiaoAnVehicleController));

        /// <summary>
        /// 开车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool Open(string vehicleid, out string returnResult)
        {
            returnResult = "";
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht= vm.GetVehicleInfoByID(vehicleid);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("type",1);
                dict.Add("command", 90);
                dict.Add("cmd", "carControl");
                dict.Add("directRt","false");
                //string sign = ZCloudSignUtil.getSign(dict, XiaoAnCloudConfig.md5_key);
                //dict.Add("sign",sign);
                //string postData=ZCloudCore.createLinkString(dict);
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = PostJson(XiaoAnCloudConfig.api_url, postData, "utf-8");
                returnResult = retstr;
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                {
                    result = true;
                }
                Logger.Debug("开锁:"+ "carId:"+ dict["carId"] +","+ retstr);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("开锁异常:"+ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 锁车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool Close(string vehicleid, out string returnResult)
        {
            returnResult = "";
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("type", 1);
                dict.Add("command", 4);
                dict.Add("cmd", "carControl");
                dict.Add("directRt", "false");
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = PostJson(XiaoAnCloudConfig.api_url, postData, "utf-8");
                returnResult = retstr;
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                {
                    /*
                    //播放还车语音
                    dict = new Dictionary<string, object>();
                    dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                    dict.Add("type", 11);
                    dict.Add("command", 14);
                    dict.Add("cmd", "carControl");
                    dict.Add("directRt", "false");
                    postData = JsonConvert.SerializeObject(dict);
                    retstr = PostJson(XiaoAnCloudConfig.api_url, postData, "utf-8");
                    */
                    result = true;
                }
                Logger.Debug("锁车:" + "carId:" + dict["carId"] + "," + retstr);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("锁车异常:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 寻车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool Find(string vehicleid,out string returnResult)
        {
            returnResult = "";
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("type", 5);
                dict.Add("command", 14);
                dict.Add("cmd", "carControl");
                dict.Add("directRt", "false");
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = PostJson(XiaoAnCloudConfig.api_url, postData, "utf-8");
                returnResult = retstr;
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                {
                    result = true;
                }
                Logger.Debug("寻车:" + "carId:" + dict["carId"] + "," + retstr);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("寻车异常:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 开座垫指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool OpenSeat(string vehicleid, out string returnResult)
        {
            returnResult = "";
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("type", 1);
                dict.Add("command", 29);
                dict.Add("cmd", "carControl");
                dict.Add("directRt", "false");
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = PostJson(XiaoAnCloudConfig.api_url, postData, "utf-8");
                returnResult = retstr;
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                {
                    result = true;
                }
                Logger.Debug("开座垫:" + "carId:" + dict["carId"] + "," + retstr);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("开座垫异常:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 车辆断电指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool PowerOff(string vehicleid, out string returnResult)
        {
            returnResult = "";
            try
            {
                bool result = false;
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("carId", SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum"));
                dict.Add("type", 0);
                dict.Add("command", 90);
                dict.Add("cmd", "carControl");
                dict.Add("directRt", "false");
                string postData = JsonConvert.SerializeObject(dict);
                string retstr = PostJson(XiaoAnCloudConfig.api_url, postData, "utf-8");
                returnResult = retstr;
                dynamic retObj = DynamicJson.Parse(retstr);
                if (retObj.rtCode == "0")
                {
                    result = true;
                }
                Logger.Debug("断电:" + "carId:" + dict["carId"] + "," + retstr);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("断电异常:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 开电池锁指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool OpenBatteryLock(string vehicleid, out string returnResult)
        {
            returnResult = "";
            return false;
        }

        /// <summary>
        /// 关电池锁指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public bool CloseBatteryLock(string vehicleid, out string returnResult)
        {
            returnResult = "";
            return false;
        }

        private string PostJson(string url, string postDataStr, string codename)
        {
            Encoding encoding = Encoding.GetEncoding(codename);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] bs = encoding.GetBytes(postDataStr);
            request.ContentLength = bs.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(bs, 0, bs.Length);
            myRequestStream.Flush();
            myRequestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
    }
}
