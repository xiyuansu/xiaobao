using System;
using System.Collections.Generic;
using System.Text;

namespace Asiasofti.SmartVehicle.Common.Enum
{
    /// <summary>
    /// 图片类型
    /// </summary>
    public enum ImagesType : int
    {
        /// <summary>
        /// 缩略图
        /// </summary>
        [EnumShowName("缩略图")]
        Thumbnail = 1,

        /// <summary>
        /// 详情图
        /// </summary>
        [EnumShowName("详情图")]
        Details = 2,

        /// <summary>
        /// 晒图
        /// </summary>
        [EnumShowName("晒图")]
        Print = 3,

        /// <summary>
        /// 其他
        /// </summary>
        [EnumShowName("其它")]
        Other = 4

    }
}
