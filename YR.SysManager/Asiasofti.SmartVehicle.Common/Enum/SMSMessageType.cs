using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 短信发送者类型
    /// </summary>
    public enum SMSMessageType : int
    {
        /// <summary>
        /// 系统发送
        /// </summary>
        [EnumShowName("系统发送")]
        System = 1,

        /// <summary>
        /// 会员发送
        /// </summary>
        [EnumShowName("会员发送")]
        User = 2,

        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Ohter = 3
    }
}
