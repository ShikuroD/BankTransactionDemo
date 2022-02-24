using System.Collections.Generic;
using System;

namespace BankTransactionConsole.Models
{
    public class AccountSummary
    {
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }


        public AccountSummary(int accountNumber, decimal balance)
        {
            AccountNumber = accountNumber;
            Balance = balance;
        }

        public AccountSummary()
        {
        }
    }
}
