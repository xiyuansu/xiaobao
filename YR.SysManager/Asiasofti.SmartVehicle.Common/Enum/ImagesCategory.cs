using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 图片种类
    /// </summary>
    public enum ImagesCategory : int
    {
        /// <summary>
        /// 车辆
        /// </summary>
        [EnumShowName("车辆")]
        Vehicle = 1,

        /// <summary>
        /// 充电桩
        /// </summary>
        [EnumShowName("充电桩")]
        ChargingPie = 2,

        /// <summary>
        /// 维修站
        /// </summary>
        [EnumShowName("维修站")]
        Pits = 3,

        /// <summary>
        /// 用户头像
        /// </summary>
        [EnumShowName("用户头像")]
        UserHead = 4,

        /// <summary>
        /// 用户身份证
        /// </summary>
        [EnumShowName("用户身份证")]
        UserIdCard = 5,

        /// <summary>
        /// 用户身份证
        /// </summary>
        [EnumShowName("用户驾驶证")]
        UserDriving = 6,

        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Other = 20
    }
}
