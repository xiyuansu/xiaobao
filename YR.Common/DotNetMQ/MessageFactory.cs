using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetMQ
{
    /// <summary>
    /// 消息队列工厂
    /// </summary>
    public class MessageFactory
    {
        /// <summary>
        /// 获取消息服务
        /// </summary>
        /// <returns></returns>
        public static IMessage GetMessage()
        {
            return new RabbitMq();
        }
    }
}
