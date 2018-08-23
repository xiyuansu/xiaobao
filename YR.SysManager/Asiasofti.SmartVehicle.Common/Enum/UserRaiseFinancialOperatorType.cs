using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 会员众筹财务操作类型
    /// </summary>
    public enum UserRaiseFinancialOperatorType : int
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EnumShowName("用户")]
        User = 1,

        /// <summary>
        /// 管理员
        /// </summary>
        [EnumShowName("管理员")]
        Admin = 2
    }
}
