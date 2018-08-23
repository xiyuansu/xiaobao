using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 车辆使用状态 1空闲,2预约中,3客户使用中,4运维操作中
    /// </summary>
    public enum VehicleUseState : int
    {
        /// <summary>
        /// 空闲
        /// </summary>
        [EnumShowName("空闲")]
        Free = 1,

        /// <summary>
        /// 预约中
        /// </summary>
        [EnumShowName("预约中")]
        Reservation = 3,

        /// <summary>
        /// 客户使用中
        /// </summary>
        [EnumShowName("客户使用中")]
        Order = 2,

        /// <summary>
        /// 运维操作中
        /// </summary>
        [EnumShowName("运维操作中")]
        Operations = 4,
    }
}
