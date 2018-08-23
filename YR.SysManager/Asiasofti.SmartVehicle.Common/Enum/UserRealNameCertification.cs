using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 用户提交众筹申请状态
    /// </summary>
    public enum UserRealNameCertification : int
    {
        /// <summary>
        /// 未认证
        /// </summary>
        [EnumShowName("未认证")]
        Unauthorized = 1,

        /// <summary>
        /// 提交申请
        /// </summary>
        [EnumShowName("提交申请")]
        SubmitApply = 2,
        
        /// <summary>
        /// 认证失败
        /// </summary>
        [EnumShowName("认证失败")]
        AuthFailed = 3,

        /// <summary>
        /// 已认证
        /// </summary>
        [EnumShowName("已认证")]
        Certified = 4

    }
}
