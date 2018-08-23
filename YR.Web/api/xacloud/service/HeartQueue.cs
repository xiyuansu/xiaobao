using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Web;

namespace YR.Web.api.xacloud.service
{
    public class HeartQueue : IDisposable
    {
        //指定消息队列事务的类型
        protected MessageQueueTransactionType transactionType = MessageQueueTransactionType.Automatic;
        protected MessageQueue queue;  //消息队列
        protected TimeSpan timeout;    //时间间隔

        public HeartQueue(string queuePath, int timeoutSeconds)
        {
            queue = new MessageQueue(queuePath);  //根据传入quueuPath创建队列
            timeout = TimeSpan.FromSeconds(Convert.ToDouble(timeoutSeconds));

            queue.DefaultPropertiesToSend.AttachSenderId = false;
            queue.DefaultPropertiesToSend.UseAuthentication = false;
            queue.DefaultPropertiesToSend.UseEncryption = false;
            queue.DefaultPropertiesToSend.AcknowledgeType = AcknowledgeTypes.None;
            queue.DefaultPropertiesToSend.UseJournalQueue = false;
        }

        /// <summary>
        /// 接收消息方法
        /// </summary>
        public virtual object Receive()
        {
            try
            {
                using (Message message = queue.Receive(timeout, transactionType))
                    return message;
            }
            catch (MessageQueueException mqex)
            {
                if (mqex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    throw new TimeoutException();
                throw;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public virtual void Send(object msg)
        {
            queue.Send(msg, transactionType);
        }

        #region IDisposable 成员
        public void Dispose()
        {
            queue.Dispose();  //解放资源
        }
        #endregion
    }
}