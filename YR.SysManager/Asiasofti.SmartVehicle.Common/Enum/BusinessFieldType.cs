using System;
using System.Collections.Generic;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 业务字段类型
    /// </summary>
    public enum BusinessFieldType : int
    {
        /// <summary>
        /// 车辆介绍
        /// </summary>
        [EnumShowName("VEHICLEREMARK")]
        VEHICLEREMARK = 1,

        /// <summary>
        /// 价格体系
        /// </summary>
        [EnumShowName("PRICESYSTEM")]
        PRICESYSTEM = 2,

        /// <summary>
        /// 用户须知
        /// </summary>
        [EnumShowName("TRANSPORTINFORMATION")]
        TRANSPORTINFORMATION = 3,

        /// <summary>
        /// 描述（充电桩、维修站）
        /// </summary>
        [EnumShowName("DESCRIPTION")]
        DESCRIPTION = 4
    }
}
