using System.Transactions;
using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BankTransactionConsole
{
    public class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        public string ExchangeName { get; set; } = "exchange_demo";

        public RpcClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: "transaction",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueBind(queue: "transaction",
                              exchange: "exchange_demo",
                              routingKey: "transaction_route");

            channel.QueueDeclare(
                queue: "reply",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueBind(queue: "reply",
                            exchange: "exchange_demo",
                            routingKey: "reply_route");
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);
        }

        public string Call(string routingKey, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "exchange_demo",
                routingKey: routingKey,
                basicProperties: props,
                body: messageBytes);

            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }

    public class RoutingKey
    {
        public static readonly string IndentityKey = "identity";
        public static readonly string TransactionKey = "transaction";
    }
}
