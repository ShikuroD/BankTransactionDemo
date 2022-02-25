using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Transaction.APIcopy.DTOs;
using Transaction.APIcopy.Models;
using Transaction.APIcopy.Services;

namespace Transaction.APIcopy.Helpers {
    public class Processor : IHostedService {
        private readonly ITransactionService _service;
        private readonly IConfiguration _config;
        private Consumer _consumer;
        public Processor (IServiceScopeFactory factory, IConfiguration config) {
            _config = config;
            _service = factory.CreateScope ().ServiceProvider.GetRequiredService<ITransactionService> ();
            _consumer = new Consumer ();
        }
        public Task StartAsync (CancellationToken cancellationToken) {

            RunConsumer ();
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
        public void RunConsumer () {
            var receiver = new EventingBasicConsumer (_consumer._channel);

            _consumer._channel.BasicConsume (queue: "transaction",
                autoAck : false,
                consumer : receiver);

            receiver.Received += (model, ea) => {
                var body = ea.Body.ToArray ();
                var props = ea.BasicProperties;
                var replyProps = _consumer._channel.CreateBasicProperties ();
                replyProps.CorrelationId = props.CorrelationId;
                TransactionMessage response = null;
                try {
                    string message = Encoding.UTF8.GetString (body);
                    response = ProcessMessage (message).Result;
                    Console.WriteLine (String.Format (">    Reply processed: {0}", response is null? "null": response.ToString ()));
                } catch (Exception e) {
                    Console.WriteLine (e.ToString ());
                } finally {
                    var responseBytes = Encoding.UTF8.GetBytes (response is null? "": response.ToString ());
                    _consumer._channel.BasicPublish (exchange: "exchange_demo", routingKey : props.ReplyTo,
                        basicProperties : replyProps, body : responseBytes);

                    _consumer._channel.BasicAck (deliveryTag: ea.DeliveryTag,
                        multiple: false);
                    Console.WriteLine (">    Replied\n\n");
                }
            };

        }
        private async Task<TransactionMessage> ProcessMessage (string message) {
            Console.WriteLine (">   Message received: " + message.ToString ());
            TransactionMessage myMessage = null;
            if (!String.IsNullOrEmpty (message))
                myMessage = JsonConvert.DeserializeObject<TransactionMessage> (message);

            AccountTransactionDto dto = JsonConvert.DeserializeObject<AccountTransactionDto> (myMessage.Content);
            using (var client = new HttpClient ()) {
                string response = null;
                client.DefaultRequestHeaders.Accept.Clear ();
                client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                client.DefaultRequestHeaders.Add ("Authorization", "Bearer " + myMessage.Token);
                var baseUri = $"http://localhost:5003/api/account";
                try {
                    switch (myMessage.Code) {
                        case MessageCode.Balance:
                            response = JsonConvert.SerializeObject (await GetBalance (client, baseUri + "/balance"));
                            break;
                        case MessageCode.Deposit:
                            response = JsonConvert.SerializeObject (await ExecuteTransaction (client, baseUri + "/deposit", dto));
                            break;
                        case MessageCode.Withdraw:
                            response = JsonConvert.SerializeObject (await ExecuteTransaction (client, baseUri + "/withdraw", dto));
                            break;
                    }
                } catch (Exception ex) {
                    Console.WriteLine (ex.ToString ());
                    return new TransactionMessage (myMessage.Token, myMessage.Code, response);
                }
                return new TransactionMessage (myMessage.Token, myMessage.Code, response);
            }

        }

        public Task StopAsync (CancellationToken cancellationToken) {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        private async Task<AccountSummary> GetBalance (HttpClient client, string baseUri) {
            var response = await client.GetAsync (baseUri);

            if (response.StatusCode != HttpStatusCode.OK) {
                Console.WriteLine ($"Error {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
            var repString = await response.Content.ReadAsStringAsync ();
            Console.WriteLine ($">    API call result: {repString}");
            return JsonConvert.DeserializeObject<AccountSummary> (repString);
        }

        private async Task<AccountTransactionResponse> ExecuteTransaction (HttpClient client, string baseUri, AccountTransactionDto tranx) {
            var response = await client.PostAsJsonAsync (baseUri, tranx);

            if (response.StatusCode != HttpStatusCode.OK) {
                Console.WriteLine ($"Error {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
            var repString = await response.Content.ReadAsStringAsync ();
            Console.WriteLine ($">    API call result: {repString}");
            return JsonConvert.DeserializeObject<AccountTransactionResponse> (repString);
        }

    }
}