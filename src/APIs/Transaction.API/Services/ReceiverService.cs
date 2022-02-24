using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;

namespace Transaction.API.Services
{
    public class ReceiverService : IHostedService
    {
        private readonly ISubscriber subscriber;
        public ReceiverService(ISubscriber subscriber)
        {
            this.subscriber = subscriber;

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(ProcessMessage);
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
        private bool ProcessMessage(string message, IDictionary<string, object> headers)
        {
            Console.WriteLine("Transaction: " + message);
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}
