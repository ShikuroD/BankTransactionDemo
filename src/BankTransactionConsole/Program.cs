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

        static RpcClient rpcClient = new RpcClient();

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

        static async Task<AccountSummary> GetBalanceMQ()
        {
            var message = new TransactionMessage(client.DefaultRequestHeaders.Authorization.ToString(),
                                        MessageCode.Balance,
                                        JsonConvert.SerializeObject(null));
            var response = rpcClient.Call("transaction_route", JsonConvert.SerializeObject(message));
            var repMessage = JsonConvert.DeserializeObject<TransactionMessage>(response);
            return JsonConvert.DeserializeObject<AccountSummary>(repMessage.Content);
        }
        static async Task<AccountTransactionResponse> DepositMQ(AccountTransaction tranx)
        {
            var message = new TransactionMessage(client.DefaultRequestHeaders.Authorization.ToString(),
                                        MessageCode.Deposit,
                                        JsonConvert.SerializeObject(tranx));
            var response = rpcClient.Call("transaction_route", JsonConvert.SerializeObject(message));
            var repMessage = JsonConvert.DeserializeObject<TransactionMessage>(response);
            return JsonConvert.DeserializeObject<AccountTransactionResponse>(repMessage.Content);
        }
        static async Task<AccountTransactionResponse> WithdrawMQ(AccountTransaction tranx)
        {
            var message = new TransactionMessage(client.DefaultRequestHeaders.Authorization.ToString(),
                                        MessageCode.Withdraw,
                                        JsonConvert.SerializeObject(tranx));
            var response = rpcClient.Call("transaction_route", JsonConvert.SerializeObject(message));
            var repMessage = JsonConvert.DeserializeObject<TransactionMessage>(response);
            return JsonConvert.DeserializeObject<AccountTransactionResponse>(repMessage.Content);
        }

        static void Main(string[] args)
        {
            //Program ruuner = new Program();
            RunAync().Wait();
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

            var summary = await GetBalanceMQ();
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
                transactionResult = await DepositMQ(transactionInput);
            }
            else if(transactionType == TransactionType.Withdraw)
            {
                Console.WriteLine("Withdraw process starting");
                transactionResult = await WithdrawMQ(transactionInput);
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
    }
}
