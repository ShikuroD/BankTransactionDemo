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
    public class TaskQueueTest : IHostedService
    {
        private readonly ITransactionService _service;
        private Consumer _consumer {get; set;}
        private readonly IConfiguration _config;
        public TaskQueueTest(IServiceScopeFactory factory, ITransactionService service, IConfiguration config)
        {
            _config = config;
            _service = service;
            _consumer = new Consumer();
            _consumer.QueueBind("exchange_demo","queue_todo","todo.*");
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
                var body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                ProcessMessage(message);
            };
            _consumer._channel.BasicConsume(queue: "queue_todo", 
                                autoAck: true,
                                consumer: consumer);
        }
        private bool ProcessMessage(string message)
        {
            Console.WriteLine("task queue: " + message.ToString());
            TransactionMessage myMessage = JsonConvert.DeserializeObject<TransactionMessage>(message);
            
            // if(myMessage != null && myMessage.type == "post")
            return true;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}