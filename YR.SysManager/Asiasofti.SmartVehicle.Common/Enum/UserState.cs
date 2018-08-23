using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserState : int
    {
        /// <summary>
        /// 禁用
        /// </summary>
        [EnumShowName("禁用")]
        Disable = 0,

        /// <summary>
        /// 启用
        /// </summary>
        [EnumShowName("启用")]
        Enable = 1
    }
}
