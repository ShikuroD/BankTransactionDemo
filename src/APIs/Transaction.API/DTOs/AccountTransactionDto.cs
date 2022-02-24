using System;
using Transaction.API.Models;

namespace Transaction.API.DTOs
{
    public class AccountTransactionDto
    {
        public int TransactionId { get; set; }
        public int AccountNumber { get; set; }
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public AccountTransactionDto(int transactionId, int accountNumber, DateTime date, TransactionType transactionType, string description, decimal amount)
        {
            TransactionId = transactionId;
            AccountNumber = accountNumber;
            Date = date;
            TransactionType = transactionType;
            Description = description;
            Amount = amount;
        }

        public AccountTransactionDto()
        {
        }
    }

    public class AccountTransactionResponse
    {
        public string Message { get; set; }
        public decimal CurrentBalance { get; set; }
        public AccountTransactionDto Transaction { get; set; }

        public AccountTransactionResponse(string message, decimal currentBalance, AccountTransactionDto transaction)
        {
            Message = message;
            CurrentBalance = currentBalance;
            Transaction = transaction;
        }

        public AccountTransactionResponse()
        {
        }
    }
}
