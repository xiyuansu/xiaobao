using System;
using System.Collections.Generic;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 业务类型
    /// </summary>
    public enum BusinessType : int
    {
        /// <summary>
        /// 车辆
        /// </summary>
        [EnumShowName("车辆")]
        Vehicle = 1,

        /// <summary>
        /// 维修站
        /// </summary>
        [EnumShowName("维修站")]
        Pits = 2,

        /// <summary>
        /// 停车场/充电桩
        /// </summary>
        [EnumShowName("停车场")]
        ChargingPies = 3
    }
}
