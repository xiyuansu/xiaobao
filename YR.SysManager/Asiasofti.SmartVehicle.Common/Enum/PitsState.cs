using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 维修站状态
    /// </summary>
    public enum PitsState
    {
        /// <summary>
        /// 可用
        /// </summary>
        [EnumShowName("可用")]
        Enable=1,

        /// <summary>
        /// 不可用
        /// </summary>
        [EnumShowName("不可用")]
        Disabled=2
    }
}
