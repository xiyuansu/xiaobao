using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    public enum UserRaiseReplyState : int
    {
        /// <summary>
        /// 新提交
        /// </summary>
        [EnumShowName("新提交")]
        NewSubmission = 1,

        /// <summary>
        /// 审核成功
        /// </summary>
        [EnumShowName("审核成功")]
        ReviewSuccess = 2,

        /// <summary>
        /// 审核失败
        /// </summary>
        [EnumShowName("审核失败")]
        ReviewFaild = 3
    }
}
