using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YR.Web.api.api_class;

namespace YR.Web.api.api_class
{
    /// <summary>
    /// api方法接口
    /// </summary>
    public interface IApiAction2
    {
        /// <summary>
        /// 接口方法
        /// </summary>
        /// <param name="params_ht"></param>
        /// <returns></returns>
        string Execute(Hashtable params_ht);

    }
}
