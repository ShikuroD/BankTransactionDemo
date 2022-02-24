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
using System.Net.Http;
using Transaction.API.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace Transaction.API.Helpers
{
    public class Processor : IHostedService
    {
        private readonly ITransactionService _service;
        private Consumer _consumer { get; set; }
        private readonly IConfiguration _config;
        public Processor(IServiceScopeFactory factory, IConfiguration config)
        {
            _config = config;
            _service = factory.CreateScope().ServiceProvider.GetRequiredService<ITransactionService>();
            _consumer = new Consumer();
            _consumer.QueueBind("exchange_demo", "transaction");
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            RunConsumer();
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
        public void RunConsumer()
        {
            var consumer = new EventingBasicConsumer(_consumer._channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _consumer._channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                TransactionMessage response = null;
                try
                {
                    string message = Encoding.UTF8.GetString(body);
                    response = ProcessMessage(message).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());

                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response.ToString());
                    Console.WriteLine(String.Format("{0}\n{1}\n{2}",props.ReplyTo.ToString(),replyProps.ToString(), responseBytes.ToString()));
                    _consumer._channel.BasicPublish(exchange: "exchange_demo", routingKey: props.ReplyTo,
                      basicProperties: replyProps, body: responseBytes);
                    
                    _consumer._channel.BasicAck(deliveryTag: ea.DeliveryTag,
                      multiple: false);
                }
            };
            _consumer._channel.BasicConsume(queue: "transaction",
                                autoAck: false,
                                consumer: consumer);
        }
        private async Task<TransactionMessage> ProcessMessage(string message)
        {
            Console.WriteLine("task queue: " + message.ToString());
            TransactionMessage myMessage = null;
            if(String.IsNullOrEmpty(message))  myMessage =null;
            else
                myMessage = JsonConvert.DeserializeObject<TransactionMessage>(message);

            AccountTransactionDto dto = JsonConvert.DeserializeObject<AccountTransactionDto>(myMessage.Content);
            using (var client = new HttpClient())
            {
                AccountTransactionResponse response = null;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + myMessage.Token);
                var baseUri = $"http://localhost:5000/api/account";
                try
                {
                    switch (myMessage.Code)
                    {
                        case MessageCode.Balance:
                            response = await Execute(client, baseUri + "/balance", null);
                            break;
                        case MessageCode.Deposit:
                            if (!dto.isNull())
                            {
                                response = await Execute(client, baseUri + "/deposit", dto);
                            }
                            break;
                        case MessageCode.Withdraw:
                            if (!dto.isNull())
                            {
                                response = await Execute(client, baseUri + "/withdraw", dto);
                            }
                            break;
                    }
                    if (response is null) return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
                return new TransactionMessage(myMessage.Token, myMessage.Code, JsonConvert.SerializeObject(response));
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        private async Task<AccountTransactionResponse> Execute(HttpClient client, string baseUri, AccountTransactionDto tranx)
        {
            var uri = $"/balance";
            if(tranx is not null)
                uri = tranx.TransactionType.Equals(TransactionType.Deposit)?$"/deposit":$"/withdraw";
            var response = await client.PostAsJsonAsync(baseUri + uri, tranx);
            Console.WriteLine(response);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
            var repString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(repString);
            return JsonConvert.DeserializeObject<AccountTransactionResponse>(repString);
        }
    }
}