using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetMQ
{
    /// <summary>
    /// 消息类
    /// </summary>
    public class MessageData
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string type;

        /// <summary>
        /// 消息对象
        /// </summary>
        public object content;
    }
}
