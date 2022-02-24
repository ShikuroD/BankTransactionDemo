using System;

namespace BankTransactionConsole.Models
{
    public class AccountTransaction
    {
        public int TransactionId { get; set; }
        public int AccountNumber { get; set; }
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

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

    public class AccountTransactionResponse
    {
        public string Message { get; set; }
        public decimal CurrentBalance { get; set; }
        public AccountTransaction Transaction { get; set; }

        public AccountTransactionResponse(string message, decimal currentBalance, AccountTransaction transaction)
        {
            Message = message;
            CurrentBalance = currentBalance;
            Transaction = transaction;
        }

        public AccountTransactionResponse()
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
