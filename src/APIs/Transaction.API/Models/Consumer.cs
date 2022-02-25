using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Transaction.API.Models {
    public class Consumer {
        private ConnectionFactory _factory { get; set; }
        private IConnection _connection { get; set; }
        public IModel _channel { get; set; }
        private string queueName { get; set; }

        public Consumer () {
            _factory = new ConnectionFactory () { HostName = "localhost" };
            _connection = _factory.CreateConnection ();
            _channel = _connection.CreateModel ();
            this.InitQueue ("transaction");
            this.QueueBind ("exchange_demo", "transaction", "transaction_route");
            _channel.BasicQos (0, 1, false);
        }

        public void InitQueue (string name = "hello") {
            this.queueName = name;
            _channel.QueueDeclare (
                queue: name,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        // public string Receive (string message = "Hello world") {
        //     string response = "";
        //     var consumer = new EventingBasicConsumer (_channel);
        //     consumer.Received += (model, ea) => {
        //         var body = ea.Body.ToArray ();
        //         var message = Encoding.UTF8.GetString (body);
        //         response = message;
        //     };

        //     _channel.BasicConsume (queueName, true, consumer);
        //     Console.WriteLine ("Receive: " + response.ToString ());
        //     return response;
        // }
        public void QueueBind (string exchangeName = "", string queueName = "", string routeKey = "") {
            if (queueName.Trim () == "")
                queueName = _channel.QueueDeclare ().QueueName;
            _channel.QueueBind (queue: queueName,
                exchange: exchangeName,
                routingKey: routeKey);
        }
        // public string InitConsumer () {
        //     string message = "";
        //     var consumer = new EventingBasicConsumer (_channel);

        //     // handle the Received event on the consumer
        //     // this is triggered whenever a new message
        //     // is added to the queue by the producer
        //     consumer.Received += (model, ea) => {
        //         // read the message bytes
        //         var body = ea.Body.ToArray ();

        //         message = Encoding.UTF8.GetString (body);

        //         //Console.WriteLine(" [x] Received {0}", message);
        //     };
        //     _channel.BasicConsume (queue: this.queueName,
        //         autoAck: true,
        //         consumer: consumer);
        //     return message;
        // }
    }
}