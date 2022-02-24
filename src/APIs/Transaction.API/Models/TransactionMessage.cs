using System;
using Newtonsoft.Json;

namespace Transaction.API.Models
{
    public class TransactionMessage
    {
        public string Token { get; set; }
        public MessageCode Code { get; set; }
        public string Content { get; set; }

        public TransactionMessage(string token, MessageCode code, string content)
        {
            Token = token;
            Code = code;
            Content = content;
        }

        public TransactionMessage()
        {
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public enum MessageCode
    {
        Login,
        Balance,
        Deposit,
        Withdraw
    }
}
