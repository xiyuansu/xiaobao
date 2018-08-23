using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    public enum UserRaiseWithdrawalApplyState
    {
        /// <summary>
        /// 新提交
        /// </summary>
        [EnumShowName("新提交")]
        NewSubmit=1,
        
        /// <summary>
        /// 已经处理
        /// </summary>
        [EnumShowName("已处理")]
        AlreadyDo=2,

        /// <summary>
        /// 无效
        /// </summary>
        [EnumShowName("无效")]
        Invalid=3
    }
}
