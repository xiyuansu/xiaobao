using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    public enum OrderPayState : int
    {
        /// <summary>
        /// 未支付
        /// </summary>
        [EnumShowName("未支付")]
        NotPay = 0,

        /// <summary>
        /// 已支付
        /// </summary>
        [EnumShowName("已支付")]
        AlreadyPay = 1,

        /// <summary>
        /// 余额不足
        /// </summary>
        [EnumShowName("余额不足")]
        LessBlance = 2
    }
}
