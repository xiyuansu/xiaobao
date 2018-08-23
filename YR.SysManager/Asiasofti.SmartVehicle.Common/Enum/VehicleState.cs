using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 车辆状态 1可用,2不可用,3维修,4丢失,5其他,7故障
    /// </summary>
    public enum VehicleState : int
    {
        /// <summary>
        /// 可用
        /// </summary>
        [EnumShowName("可用")]
        Use = 1,

        /// <summary>
        /// 不可用
        /// </summary>
        [EnumShowName("不可用")]
        NoUse = 2,

        /// <summary>
        /// 维修
        /// </summary>
        [EnumShowName("维修")]
        Repair = 3,

        /// <summary>
        /// 丢失
        /// </summary>
        [EnumShowName("丢失")]
        Loss = 4,
    
        /// <summary>
        /// 其他
        /// </summary>
        [EnumShowName("其他")]
        Other = 5,

        /// <summary>
        /// 故障申报
        /// </summary>
        [EnumShowName("故障")]
        Fault = 7
    }
}
