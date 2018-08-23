using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 车辆控制类型
    /// </summary>
    public enum VehicleControlType : int
    {
        /// <summary>
        /// 保养
        /// </summary>
        [EnumShowName("车辆锁定")]
        Lock = 1,
        
        /// <summary>
        /// 维修
        /// </summary>
        [EnumShowName("车灯")]
        Light = 2,
        
        /// <summary>
        /// 其他
        /// </summary>
        [EnumShowName("其它")]
        Other = 3,

    }
}
