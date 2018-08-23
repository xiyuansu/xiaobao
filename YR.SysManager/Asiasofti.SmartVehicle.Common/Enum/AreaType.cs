using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 区域类型
    /// </summary>
    public enum AreaType : int
    {
        /// <summary>
        /// 服务范围
        /// </summary>
        [EnumShowName("服务范围")]
        Service = 1,

        /// <summary>
        /// 停车网点
        /// </summary>
        [EnumShowName("停车网点")]
        Stop = 2,

        /// <summary>
        /// 禁停区域
        /// </summary>
        [EnumShowName("禁停区域")]
        Forbid = 3,

    }
}
