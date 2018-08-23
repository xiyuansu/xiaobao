using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    public enum OrderState : int
    {
        /// <summary>
        /// 已取消
        /// </summary>
        [EnumShowName("已取消")]
        Invalid = 0,

        /// <summary>
        /// 进行中
        /// </summary>
        [EnumShowName("进行中")]
        Valid = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        [EnumShowName("已完成")]
        Finished = 2,
        
        /// <summary>
        /// 异常订单
        /// </summary>
        [EnumShowName("异常订单")]
        Abnormal = 3,

        /// <summary> 
        /// 未支付
        /// </summary>
        [EnumShowName("未支付")]
        UnPay = 5
    }
}
