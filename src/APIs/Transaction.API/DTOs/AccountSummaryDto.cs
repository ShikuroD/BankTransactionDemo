using System;

namespace Transaction.API.DTOs
{
    public class AccountSummaryDto
    {
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }


        public AccountSummaryDto(int accountNumber, decimal balance)
        {
            AccountNumber = accountNumber;
            Balance = balance;
        }

        public AccountSummaryDto()
        {
        }
    }
}
