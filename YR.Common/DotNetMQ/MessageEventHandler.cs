using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetMQ
{
    /// <summary>
    /// 消息事件参数
    /// </summary>
    public class MessageEventArgs:EventArgs
    {
        private MessageData data;

        public MessageData Data 
        { 
            get 
            {
                return data;
            }
        }

        public MessageEventArgs(MessageData data)
        {
            this.data = data;
        }
    }

    /// <summary>
    /// 消息处理事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public delegate bool MessageEventHandler(object sender, MessageEventArgs e);
}
