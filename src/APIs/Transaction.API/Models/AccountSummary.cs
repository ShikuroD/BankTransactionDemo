using System.Collections.Generic;
using System;

namespace Transaction.API.Models
{
    public class AccountSummary
    {
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }

        //DB Relation
        public IList<AccountTransaction> AccountTransactions { get; set; }

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
