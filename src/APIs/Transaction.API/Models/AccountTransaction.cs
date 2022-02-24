using System;

namespace Transaction.API.Models
{
    public class AccountTransaction
    {
        public int TransactionId { get; set; }
        public int AccountNumber { get; set; }
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        //DB Relation
        public AccountSummary AccountSummary { get; set; }

        public AccountTransaction(int transactionId, int accountNumber, DateTime date, TransactionType transactionType, string description, decimal amount)
        {
            TransactionId = transactionId;
            AccountNumber = accountNumber;
            Date = date;
            TransactionType = transactionType;
            Description = description;
            Amount = amount;
        }

        public AccountTransaction()
        {
        }
        
    }

    public enum TransactionType
    {
        Deposit,
        Withdraw,
        Balance
    }
}
