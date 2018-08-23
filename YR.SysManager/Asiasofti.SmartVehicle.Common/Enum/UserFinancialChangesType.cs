using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 会员财务变动类型
    /// </summary>
    public enum UserFinancialChangesType:int
    {
        /// <summary>
        /// 充值
        /// </summary>
        [EnumShowName("充值")]
        Recharge=1,

        /// <summary>
        /// 消费
        /// </summary>
        [EnumShowName("消费")]
        Consumption=2,

        /// <summary>
        /// 平台增加
        /// </summary>
        [EnumShowName("平台增加")]
        PlatAdd=3,

        /// <summary>
        /// 平台减少
        /// </summary>
        [EnumShowName("平台减少")]
        PlatMinus=4,

        /// <summary>
        /// 押金
        /// </summary>
        [EnumShowName("押金")]
        Deposit = 5,

        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Other=99
    }
}
