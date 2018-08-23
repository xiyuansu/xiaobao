using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 车辆控制器接口
    /// </summary>
    public interface IVehicleController
    {
        /// <summary>
        /// 开车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        bool Open(string vehicleid,out string returnResult);

        /// <summary>
        /// 锁车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        bool Close(string vehicleid, out string returnResult);

        /// <summary>
        /// 寻车指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        bool Find(string vehicleid, out string returnResult);

        /// <summary>
        /// 开座垫指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        bool OpenSeat(string vehicleid, out string returnResult);

        /// <summary>
        /// 车辆断电指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        bool PowerOff(string vehicleid, out string returnResult);

        /// <summary>
        /// 开电池锁指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        bool OpenBatteryLock(string vehicleid, out string returnResult);

        /// <summary>
        /// 关电池锁指令
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        bool CloseBatteryLock(string vehicleid, out string returnResult);
    }
}
