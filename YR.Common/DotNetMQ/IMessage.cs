using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetMQ
{
    /// <summary>
    /// 消息队列接口
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 消息接收处理事件
        /// </summary>
        event MessageEventHandler onReceive;

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        void Send(MessageData data);

        /// <summary>
        /// 接收指定类型的消息
        /// </summary>
        /// <param name="msgtype"></param>
        void Receive(string msgtype);
    }
}
