using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 短消息类型
    /// </summary>
    public enum MessageType : int
    {
        /// <summary>
        /// 系统消息
        /// </summary>
        [EnumShowName("系统消息")]
        System = 1,

        /// <summary>
        /// 会员发送
        /// </summary>
        [EnumShowName("用户消息")]
        User = 2,

        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Ohter = 3
    }
}
