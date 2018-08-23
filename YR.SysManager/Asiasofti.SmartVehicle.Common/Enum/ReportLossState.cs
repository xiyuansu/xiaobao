using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 挂失状态
    /// </summary>
    public enum ReportLossState : int
    {
        /// <summary>
        /// 挂失
        /// </summary>
        [EnumShowName("挂失")]
        ReportLoss = 0,

        /// <summary>
        /// 解挂
        /// </summary>
        [EnumShowName("解挂")]
        NoReportLoss = 1
    }
}
