using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Web.api.baojia
{
    /// <summary>
    /// 接口返回类型
    /// </summary>
    public class ApiResp
    {
        /// <summary>
        /// 结果代码，0成功，其它值为失败
        /// </summary>
        public string code;

        /// <summary>
        /// 错误信息说明
        /// </summary>
        public string msg;

        /// <summary>
        /// 返回业务数据
        /// </summary>
        public object data;

    }
}