using System.Linq;
using System.Net;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using BankTransactionConsole.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BankTransactionConsole
{
    class Program
    {
        static string IdentityUrl = "http://localhost:5001/api";
        static string TransactionUrl = "http://localhost:5000/api";
        static HttpClient client = new HttpClient();

        static async Task<AuthenticateResponse> Authenticate(AuthenticateRequest req)
        {
            var response = await client.PostAsJsonAsync(IdentityUrl + $"/user/authenticate", new { Username = req.Username, Password = req.Password });
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
            var repString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AuthenticateResponse>(repString);

        }

        static async Task<AccountSummary> GetBalance()
        {
            var response = await client.GetAsync(String.Format(TransactionUrl + $"/account/balance"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
            var repString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(repString);
            return JsonConvert.DeserializeObject<AccountSummary>(repString);
        }
        static async Task<AccountTransactionResponse> Deposit(AccountTransaction tranx)
        {
            var response = await client.PostAsJsonAsync(TransactionUrl + $"/account/deposit", tranx);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
            var repString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(repString);
            var res =  JsonConvert.DeserializeObject<AccountTransactionResponse>(repString);
            return res;
        }
        static async Task<AccountTransactionResponse> Withdraw(AccountTransaction tranx)
        {
            var response = await client.PostAsJsonAsync(TransactionUrl + $"/account/withdraw", tranx);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
            var repString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(repString);
            return JsonConvert.DeserializeObject<AccountTransactionResponse>(repString);
        }

        static void Main(string[] args)
        {
            //Program ruuner = new Program();
            //RunAync().Wait();

            RunQueue();
        }
        
        static async Task RunAync()
        {
            Console.WriteLine("\n");
            Console.WriteLine("Bank Transaction Console");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                AuthenticateResponse authResponse = null;
                do
                {
                    var authRequest = LoginMenu();
                    authResponse = await Authenticate(authRequest);
                    if (authResponse is null)
                    {
                        Console.WriteLine("Username or password is incorrect");
                    }
                }
                while (authResponse is null);

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authResponse.Token);
                Console.WriteLine("\n Login Successfull.");
                Console.WriteLine("Name: " + authResponse.Name);
                Console.WriteLine("Account No: " + authResponse.AccountNumber);
                // foreach( var header in client.DefaultRequestHeaders.ToList())
                //     Console.WriteLine(header.Key+" | "+header.Value.ToString());

                await ProcessMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has happened:\n" + ex.ToString());
                //throw ex;
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("App closed.");
            }            

            Console.ReadLine();
        }

        static AuthenticateRequest LoginMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Awaiting login");
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            return new AuthenticateRequest(username, password);
        }
        static void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Choose one of these options:");
            Console.WriteLine("1. Balance");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Close app (X)");
            Console.WriteLine();
            Console.Write("Enter the option (number): ");
        }
        static async Task ProcessMenu()
        {
            string key;
            while ((key = Console.ReadKey().KeyChar.ToString()) != "4")
            {
                int.TryParse(key, out int keyValue);

                switch (keyValue)
                {
                    case 1:
                        await ShowBalance();
                        break;
                    case 2:
                        await MakeTransaction(TransactionType.Deposit);
                        break;
                    case 3:
                        await MakeTransaction(TransactionType.Withdraw);
                        break;
                }

                DisplayMenu();
            }
        }
        static async Task ShowBalance()
        {
            

            Console.WriteLine();
            Console.WriteLine("Balance");
            Console.WriteLine();

            var summary = await GetBalance();
            if(summary != null)
            {
                Console.WriteLine($"Account No: {summary.AccountNumber}");
                Console.WriteLine($"Balance: {summary.Balance}");
            }
            else
            {
                Console.WriteLine($"Message: Error");
            }

            Console.WriteLine();
        }
        static async Task MakeTransaction(TransactionType transactionType)
        {
            Console.WriteLine();

            Console.Write("Enter the Amount: ");
            var transactionAmount = Console.ReadLine();
            decimal number = 0;
            if (!decimal.TryParse(transactionAmount, out number))
            {
                Console.WriteLine("Invalid transaction amount");
                return;
            }

            Console.Write("Enter the description: ");
            var description = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine(transactionType.ToString());
            Console.WriteLine();

            var transactionInput = new AccountTransaction() {
                TransactionType = transactionType,
                Amount = Math.Round(Convert.ToDecimal(transactionAmount), 2),
                Description = description,
                Date = DateTime.UtcNow
            };

            var transactionResult = new AccountTransactionResponse();
            if (transactionType == TransactionType.Deposit)
            {
                Console.WriteLine("Deposit process starting");
                transactionResult = await Deposit(transactionInput);
            }
            else if(transactionType == TransactionType.Withdraw)
            {
                Console.WriteLine("Withdraw process starting");
                transactionResult = await Withdraw(transactionInput);
            }

            if(transactionResult != null && transactionResult.Transaction != null)
            {
                Console.WriteLine($"Message: {transactionResult.Message}");
                Console.WriteLine($"Account No: { transactionResult.Transaction.AccountNumber }");
                Console.WriteLine($"Current Balance: {transactionResult.CurrentBalance}");                   
            }
            else
            {
                Console.WriteLine($"Transaction failed");
                Console.WriteLine($"Message: {transactionResult.Message}");               
            }

            Console.WriteLine();
        }
        public static ConnectionFactory _factory {get; set;}
        public static IConnection _connection {get; set;}
        public static IModel _channel {get; set;}
        public static string exchangeName { get; set; } ="exchange_demo";
        public static IBasicProperties props;
        public static string correlationId;
        public static string replyQueueName ="reply_transaction";
        public static string replyRoutingkey = "reply_route_transaction";
        public static void InitConnection(){
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection(); 
            _channel = _connection.CreateModel();
            
            props = _channel.CreateBasicProperties();
            correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyRoutingkey;
        }
        public static void InitQueue(string name = "hello")
        {
            _channel.QueueDeclare(
                queue: name, 
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);
        }
        public static void InitQueueBind(string queueName, string routingKey){
            _channel.QueueBind(queue: queueName,
                              exchange: "exchange_demo",
                              routingKey: routingKey);
        }
        public static void InitExChange(){
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        }
        public static void Publish(string routeKey ="", string message ="hello world"){
            Console.WriteLine("publish " + message);
            var messageBytes = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: "transaction",
                    basicProperties: props,
                    body: messageBytes);
            //return respQueue.Take();
        }
        public static void Receive(){
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                Console.WriteLine("[x]: "+response);
                // if (ea.BasicProperties.CorrelationId == correlationId)
                // {
                //     respQueue.Add(response);
                // }
            };

            _channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);
    
        }
        public static void RunQueue(){
            Console.WriteLine("Running");
            InitConnection();
            InitExChange();
            InitQueue("queue_transaction");
            InitQueue("reply_transaction");
            InitQueueBind("queue_transaction","transaction");
            InitQueueBind("reply_transaction","reply_route_transaction");
            Receive();
            Publish("queue_transaction","from consoles");
            Console.ReadLine();
        }
    }
}
