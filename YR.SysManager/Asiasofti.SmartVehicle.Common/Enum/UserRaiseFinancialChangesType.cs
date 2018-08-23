using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 会员众筹财务变动类型
    /// </summary>
    public enum UserRaiseFinancialChangesType : int
    {
        /// <summary>
        /// 收入
        /// </summary>
        [EnumShowName("收入")]
        Income = 1,

        /// <summary>
        /// 提现
        /// </summary>
        [EnumShowName("提现")]
        Withdrawal = 2,

        /// <summary>
        /// 平台增加
        /// </summary>
        [EnumShowName("平台增加")]
        PlatAdd = 3,

        /// <summary>
        /// 平台减少
        /// </summary>
        [EnumShowName("平台减少")]
        PlatMinus = 4,
        /// <summary>
        /// 消费
        /// </summary>
        [EnumShowName("消费")]
        Consumption = 5,
        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Other = 99
    }
}
