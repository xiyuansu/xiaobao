using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserType : int
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        [EnumShowName("普通用户")]
        CommonUser = 4,

        /// <summary>
        /// 免押金用户
        /// </summary>
        [EnumShowName("免押金用户")]
        OffDepositUser = 5
    }
}
