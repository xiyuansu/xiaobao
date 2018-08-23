using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using Asiasofti.SmartVehicle.Manager;
using Asiasofti.SmartVehicle.Common.Enum;

namespace YR.Web.Manage.VehicleManage
{
    /// <summary>
    /// VehicleControl 的摘要说明
    /// </summary>
    public class VehicleControl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            string type = context.Request["type"];                      //提交动作
            string vehicleID = context.Request["id"];                    //用户主键
            bool isTrue = Convert.ToBoolean(context.Request["opr"]);
            bool Return=false;
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(context.Request["opr"]) && !string.IsNullOrEmpty(vehicleID))
            {
                string charging = context.Request["charging"];
                if (charging == "charging")
                {
                    Return = ChargControlOpr(vehicleID, type, isTrue);
                }
                else
                {
                    Return = VehicleControlOpr(vehicleID, type, isTrue);
                }
            }
            context.Response.Write(Return.ToString());
        }

        private bool VehicleControlOpr(string vehicleID, string controlType, bool isTrue)
        {
            VehicleManager vehicleManager = new VehicleManager();
            Hashtable ht= vehicleManager.GetVehicleInfoByID(vehicleID);
            string gps_ID = string.Empty;
            if (ht != null)
            {
                gps_ID = ht["VEHICLEGPSNUM"].ToString();
            }
            if (controlType == VehicleControlType.Lock.GetHashCode().ToString())
            {
                bool result = false;
                if (isTrue)
                    result = vehicleManager.CloseVehicle(vehicleID);
                else
                    result = vehicleManager.OpenVehicle(vehicleID);
                return result;
            }
            else
            {
                return false;
            }
        }

        private bool ChargControlOpr(string pointID, string controlType, bool isTrue)
        {
            ChargingPointsManager manager = new ChargingPointsManager();
            Hashtable ht= manager.GetChargingPointByID(pointID);
            string imei = string.Empty;
            if (ht != null)
            {
                imei = ht["IMEI"].ToString();
            }
            if (controlType == VehicleControlType.Lock.GetHashCode().ToString())
            {
                return manager.GPSRemoteControlLock(imei, isTrue);
            }
            else
            {
                return false;
            }
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