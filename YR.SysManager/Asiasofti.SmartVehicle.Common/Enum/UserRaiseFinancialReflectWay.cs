using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 会员众筹财务提现渠道
    /// </summary>
    public enum UserRaiseFinancialReflectWay : int
    {
        /// <summary>
        /// 银行卡
        /// </summary>
        [EnumShowName("银行卡")]
        BankCard = 1,

        /// <summary>
        /// 现金
        /// </summary>
        [EnumShowName("现金")]
        Cash = 2,
        /// <summary>
        /// 用车账户
        /// </summary>
        [EnumShowName("用车账户")]
        UseCarAccount = 3,
        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Other = 99
    }
}
