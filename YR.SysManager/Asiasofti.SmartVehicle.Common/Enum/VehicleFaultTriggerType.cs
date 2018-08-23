using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 车辆状态触发类型
    /// </summary>
    public enum VehicleFaultTriggerType : int
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EnumShowName("用户")]
        User = 1,
        /// <summary>
        /// 系统
        /// </summary>
        [EnumShowName("系统")]
        System = 2
    }
}
