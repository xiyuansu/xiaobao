using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 车辆保养/维修记录
    /// </summary>
    public enum VehicleMaintainLog : int
    {
        /// <summary>
        /// 保养
        /// </summary>
        [EnumShowName("保养")]
        Maintenance = 1,
        
        /// <summary>
        /// 维修
        /// </summary>
        [EnumShowName("维修")]
        Repair = 2,
        
        /// <summary>
        /// 其他
        /// </summary>
        [EnumShowName("其它")]
        Other = 3
    }
}
