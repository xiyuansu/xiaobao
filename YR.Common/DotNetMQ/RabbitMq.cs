using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YR.Common.DotNetConfig;

namespace YR.Common.DotNetMQ
{
    /// <summary>
    /// RabbitMq消息队列
    /// </summary>
    public class RabbitMq:IMessage,IDisposable
    {
        private ConnectionFactory factory;

        private IConnection connection;

        private IModel channel;

        public event MessageEventHandler onReceive;

        public RabbitMq()
        {
            string hostname =ConfigHelper.GetAppSettings("rabbitmq_hostname");
            string username =ConfigHelper.GetAppSettings("rabbitmq_username");
            string password =ConfigHelper.GetAppSettings("rabbitmq_password");
            string virtualhost =ConfigHelper.GetAppSettings("rabbitmq_virtualhost");
            factory = new ConnectionFactory() { HostName = hostname, VirtualHost = virtualhost, UserName = username, Password = password };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Send(MessageData data)
        {
            string message = JsonConvert.SerializeObject(data);
            var body = System.Text.Encoding.UTF8.GetBytes(message);
            channel.QueueDeclare(queue: data.type, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: "", routingKey: data.type, basicProperties: properties, body: body);
        }

        public void Receive(string msgtype)
        {
            if(onReceive!=null)
            {
                channel.QueueDeclare(queue: msgtype, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    if (onReceive != null)
                    {
                        MessageData data = JsonConvert.DeserializeObject<MessageData>(message);
                        bool result=onReceive(model, new MessageEventArgs(data));
                        if(result)
                        {
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    }
                };
                channel.BasicConsume(queue: msgtype, noAck: false, consumer: consumer);
            }
            else
            {

            }
        }

        public void Dispose()
        {
            if(channel!=null && channel.IsOpen)
            {
                channel.Close();
            }
            channel = null;
            if(connection!=null && connection.IsOpen)
            {
                connection.Close();
            }
            connection = null;
            factory = null;
        }
    }
}
