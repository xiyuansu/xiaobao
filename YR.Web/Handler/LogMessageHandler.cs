using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetLog;
using YR.Common.DotNetMQ;

namespace YR.Web.Handler
{
    public class LogMessageHandler
    {
        private Log Logger = LogFactory.GetLogger(typeof(LogMessageHandler));

        private IMessage message;

        public LogMessageHandler()
        {
            message = MessageFactory.GetMessage();
            message.onReceive += message_onReceive;
            message.Receive("001");
        }

        private bool message_onReceive(object sender, MessageEventArgs e)
        {
            bool result = false;
            try
            {
                Logger.Info(e);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}