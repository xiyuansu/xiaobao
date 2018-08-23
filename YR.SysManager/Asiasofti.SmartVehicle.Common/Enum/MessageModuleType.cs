using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 短消息模块类型
    /// </summary>
    public enum MessageModuleType : int
    {
        /// <summary>
        /// 用车模块
        /// </summary>
        [EnumShowName("用车模块")]
        UseVehicle = 1,

        /// <summary>
        /// 众筹模块
        /// </summary>
        [EnumShowName("众筹模块")]
        Raise = 2,

        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Ohter = 3,

        /// <summary>
        /// 全部
        /// </summary>
        [EnumShowName("全部")]
        All = -1
    }
}
