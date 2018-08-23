using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 评论审核状态
    /// </summary>
    public enum VehicleCommentsState
    {
        /// <summary>
        /// 新提交
        /// </summary>
        NewSubmit = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        CheckedSuccess = 2,
        /// <summary>
        /// 审核失败
        /// </summary>
        CheckedFaild = 3
    }
}
