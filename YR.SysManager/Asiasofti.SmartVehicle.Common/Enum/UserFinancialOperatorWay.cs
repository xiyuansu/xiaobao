using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 会员财务记录操作渠道
    /// </summary>
    public enum UserFinancialOperatorWay : int
    {
        /// <summary>
        /// 支付宝
        /// </summary>
        [EnumShowName("支付宝")]
        Alipay = 1,

        /// <summary>
        /// 微信支付
        /// </summary>
        [EnumShowName("微信支付")]
        WeixinPay = 2,

        /// <summary>
        /// 平台
        /// </summary>
        [EnumShowName("平台")]
        Plat = 3,

        /// <summary>
        /// 银联支付
        /// </summary>
        [EnumShowName("银联支付")]
        UnionPay = 4,

        /// <summary>
        /// 微信公众号支付
        /// </summary>
        [EnumShowName("微信公众号支付")]
        WeixinPubPay = 5,

        /// <summary>
        /// 其它
        /// </summary>
        [EnumShowName("其它")]
        Other = 99
    }
}
