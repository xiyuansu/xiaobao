using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    public enum SMSMessageState : int
    {
        /// <summary>
        /// 未发送
        /// </summary>
        [EnumShowName("未发送")]
        NoSend = 0,

        /// <summary>
        /// 已经发送
        /// </summary>
        [EnumShowName("已发送")]
        AlreadySend = 1,

        /// <summary>
        /// 发送失败
        /// </summary>
        [EnumShowName("发送失败")]
        SendFaild = 2,

        /// <summary>
        /// 已经回执
        /// </summary>
        [EnumShowName("已经回执")]
        AlreadyReceipt = 3
    }
}
