using System;

namespace Transaction.API.Models
{
    public class TransactionMessage
    {
        public string Token { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Content { get; set; }

        public TransactionMessage(string token, TransactionType transactionType, string content)
        {
            Token = token;
            TransactionType = transactionType;
            Content = content;
        }

        public TransactionMessage()
        {
        }
    }
}
