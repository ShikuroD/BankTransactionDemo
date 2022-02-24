using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Plain.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Transaction.API.Services;
using Transaction.API.Models;

namespace Transaction.API.Helpers
{
    public class Processor : IHostedService
    {
        private readonly ITransactionService _service;
        private Consumer _consumer {get; set;}
        private readonly IConfiguration _config;
        public Processor(IServiceScopeFactory factory, IConfiguration config)
        {
            _config = config;
            _service = factory.CreateScope().ServiceProvider.GetRequiredService<ITransactionService>();;
            _consumer = new Consumer();
            //_consumer.QueueBind("exchange_demo","queue_todo","todo.*");
            Console.WriteLine("background: ");
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            RunConsumer();
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
        public void RunConsumer(){
            var consumer = new EventingBasicConsumer(_consumer._channel);
            consumer.Received += (model, ea) =>
            {
                string response = "";
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _consumer._channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                string message = Encoding.UTF8.GetString(body);
                response = ProcessMessage(message);
                var responseBytes = Encoding.UTF8.GetBytes(response);
                _consumer._channel.BasicPublish(exchange: "exchange_demo", routingKey: props.ReplyTo,
                      basicProperties: replyProps, body: responseBytes);
            };
            _consumer._channel.BasicConsume(queue: "queue_transaction", 
                                autoAck: true,
                                consumer: consumer);
        }
        private string ProcessMessage(string message)
        {
            Console.WriteLine("task queue: " + message.ToString());
            //TransactionMessage myMessage = JsonConvert.DeserializeObject<TransactionMessage>(message);
            
            // if(myMessage != null && myMessage.type == "post")
            return "Test ok, connect success";
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}