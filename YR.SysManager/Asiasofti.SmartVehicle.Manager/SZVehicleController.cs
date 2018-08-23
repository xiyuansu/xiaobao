using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YR.Common.DotNetBean;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 深圳平台控制器
    /// </summary>
    public class SZVehicleController : IVehicleController
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
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht= vm.GetVehicleInfoByID(vehicleid);

                string vehicleGPSNum = SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum");

                bool returnTrue = false;
                byte[] sendcmd = new byte[13];
                //数据包类型两位
                sendcmd[0] = 0x68;
                sendcmd[1] = 0x68;
                //命令类型一位
                sendcmd[2] = 0x01;//开
                //数据包长度,数据包长度2位+IMIE号8位
                int datalen = 10;
                string strlen = datalen.ToString("X4");
                TcpClient client = new TcpClient();
                byte[] bytelen = client.strToToHexByte(strlen);
                byte[] byteimie = client.StringToBytes(vehicleGPSNum);
                Array.Copy(bytelen, 0, sendcmd, 3, 2);
                Array.Copy(byteimie, 0, sendcmd, 5, 8);

                string serverIp = SiteHelper.GetAppsetString("server_ip");
                string serverPort = SiteHelper.GetAppsetString("server_port");
                if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                {
                    if (!string.IsNullOrEmpty(vehicle_ht["BIZSOCKETIP"].ToString()))
                        serverIp = vehicle_ht["BIZSOCKETIP"].ToString();
                    if (!string.IsNullOrEmpty(vehicle_ht["BIZSOCKETPORT"].ToString()))
                        serverPort = vehicle_ht["BIZSOCKETPORT"].ToString();
                }
                bool result = client.Connect(serverIp, int.Parse(serverPort));
                if (result)
                {
                    client.Send(sendcmd);
                    string response = client.ReceiveString();
                    if (response != null && response.Contains("ok"))
                    {
                        returnTrue = true;
                    }
                    client.Disconnect();
                }
                return returnTrue;
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
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);

                string vehicleGPSNum = SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum");

                bool returnTrue = false;
                byte[] sendcmd = new byte[13];
                //数据包类型两位
                sendcmd[0] = 0x68;
                sendcmd[1] = 0x68;
                //命令类型一位
                sendcmd[2] = 0x02;//关
                //数据包长度,数据包长度2位+IMIE号8位
                int datalen = 10;
                string strlen = datalen.ToString("X4");
                TcpClient client = new TcpClient();
                byte[] bytelen = client.strToToHexByte(strlen);
                byte[] byteimie = client.StringToBytes(vehicleGPSNum);
                Array.Copy(bytelen, 0, sendcmd, 3, 2);
                Array.Copy(byteimie, 0, sendcmd, 5, 8);

                string serverIp = SiteHelper.GetAppsetString("server_ip");
                string serverPort = SiteHelper.GetAppsetString("server_port");
                if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                {
                    if (!string.IsNullOrEmpty(vehicle_ht["BIZSOCKETIP"].ToString()))
                        serverIp = vehicle_ht["BIZSOCKETIP"].ToString();
                    if (!string.IsNullOrEmpty(vehicle_ht["BIZSOCKETPORT"].ToString()))
                        serverPort = vehicle_ht["BIZSOCKETPORT"].ToString();
                }
                bool result = client.Connect(serverIp, int.Parse(serverPort));
                if (result)
                {
                    client.Send(sendcmd);
                    string response = client.ReceiveString();
                    if (response != null && response.Contains("ok"))
                    {
                        returnTrue = true;
                    }
                    client.Disconnect();
                }
                return returnTrue;
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
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);

                string vehicleGPSNum = SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum");

                bool returnTrue = false;
                byte[] sendcmd = new byte[13];
                //数据包类型两位
                sendcmd[0] = 0x68;
                sendcmd[1] = 0x68;
                //命令类型一位
                sendcmd[2] = 0x05;//寻车
                //数据包长度,数据包长度2位+IMIE号8位
                int datalen = 10;
                string strlen = datalen.ToString("X4");
                TcpClient client = new TcpClient();
                byte[] bytelen = client.strToToHexByte(strlen);
                byte[] byteimie = client.StringToBytes(vehicleGPSNum);
                Array.Copy(bytelen, 0, sendcmd, 3, 2);
                Array.Copy(byteimie, 0, sendcmd, 5, 8);

                string serverIp = SiteHelper.GetAppsetString("server_ip");
                string serverPort = SiteHelper.GetAppsetString("server_port");
                if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                {
                    if (!string.IsNullOrEmpty(vehicle_ht["BIZSOCKETIP"].ToString()))
                        serverIp = vehicle_ht["BIZSOCKETIP"].ToString();
                    if (!string.IsNullOrEmpty(vehicle_ht["BIZSOCKETPORT"].ToString()))
                        serverPort = vehicle_ht["BIZSOCKETPORT"].ToString();
                }
                bool result = client.Connect(serverIp, int.Parse(serverPort));
                if (result)
                {
                    client.Send(sendcmd);
                    string response = client.ReceiveString();
                    if (response != null && response.Contains("ok"))
                    {
                        returnTrue = true;
                    }
                    client.Disconnect();
                }
                return returnTrue;
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
