using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.APIcopy.Models;

namespace Transaction.APIcopy.Data
{

    public class SeedData
    {
        public static async Task InitializeAsync(TransactionContext context)
        {
            context.Database.EnsureCreated();

            //seed AccountSummaries
            if (!context.AccountSummaries.Any())
            {
                var AccountSummaries = new List<AccountSummary> {
                    new AccountSummary { Balance = 1000000 },
                    new AccountSummary { Balance = 2000000 },
                    new AccountSummary { Balance = 1500000 }
                };
                await context.AccountSummaries.AddRangeAsync(AccountSummaries);
                await context.SaveChangesAsync();
            }

            if (!context.AccountTransactions.Any())
            {
                var AccountTransactions = new List<AccountTransaction> {
                    new AccountTransaction {
                        AccountNumber = 2,
                        Date = new DateTime(2022,1,10,15,4,0),
                        TransactionType = TransactionType.Withdraw,
                        Description = "Withdraw",
                        Amount = 12000
                    },
                    new AccountTransaction {
                        AccountNumber = 1,
                        Date = new DateTime(2022,1,12,15,4,0),
                        TransactionType = TransactionType.Withdraw,
                        Description = "Withdraw",
                        Amount = 35000
                    },
                    new AccountTransaction {
                        AccountNumber = 2,
                        Date = new DateTime(2022,1,13,15,4,0),
                        TransactionType = TransactionType.Deposit,
                        Description = "Deposit",
                        Amount = 50000
                    },
                    new AccountTransaction {
                        AccountNumber = 3,
                        Date = new DateTime(2022,1,13,18,4,0),
                        TransactionType = TransactionType.Withdraw,
                        Description = "Withdraw",
                        Amount = 14000
                    },
                    new AccountTransaction {
                        AccountNumber = 2,
                        Date = new DateTime(2022,1,15,15,4,0),
                        TransactionType = TransactionType.Withdraw,
                        Description = "Withdraw",
                        Amount = 10000
                    },
                    new AccountTransaction {
                        AccountNumber = 3,
                        Date = new DateTime(2022,1,16,15,4,0),
                        TransactionType = TransactionType.Deposit,
                        Description = "Deposit",
                        Amount = 100000
                    },
                    new AccountTransaction {
                        AccountNumber = 1,
                        Date = new DateTime(2022,1,20,15,4,0),
                        TransactionType = TransactionType.Withdraw,
                        Description = "Withdraw",
                        Amount = 8000
                    },
                };
                await context.AccountTransactions.AddRangeAsync(AccountTransactions);
                await context.SaveChangesAsync();
            }

        }
    }
}
