using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YR.Web.api.baojia
{
    /// <summary>
    /// api方法接口
    /// </summary>
    public interface IApiAction
    {
        /// <summary>
        /// 接口方法
        /// </summary>
        /// <param name="params_ht"></param>
        /// <returns></returns>
        ApiResp Execute(Hashtable params_ht);

    }
}
