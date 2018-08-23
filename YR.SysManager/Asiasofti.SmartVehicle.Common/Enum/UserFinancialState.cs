using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 会员财务记录状态
    /// </summary>
    public enum UserFinancialState
    {
        /// <summary>
        /// 新提交
        /// </summary>
        [EnumShowName("新提交")]
        NewSubmit=1,

        /// <summary>
        /// 生效
        /// </summary>
        [EnumShowName("生效")]
        Effect=2,

        /// <summary>
        /// 无效
        /// </summary>
        [EnumShowName("无效")]
        Invalid=3
    }
}
