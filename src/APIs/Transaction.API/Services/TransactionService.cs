using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Transaction.API.Data.Repositories;
using Transaction.API.DTOs;
using Transaction.API.Exceptions;
using Transaction.API.Models;

namespace Transaction.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountSummaryRepository _accountSummaryRepos;
        private readonly IAccountTransactionRepository _accountTransactionRepos;
        private readonly IMapper _mapper;

        public TransactionService(IAccountSummaryRepository accountSummaryRepos,
                                    IAccountTransactionRepository accountTransactionRepos,
                                    IMapper mapper)
        {
            _accountSummaryRepos = accountSummaryRepos;
            _accountTransactionRepos = accountTransactionRepos;
            _mapper = mapper;
        }
        public async Task<AccountTransactionResponse> ExecuteTransaction(AccountTransaction tranx)
        {
            //get account's balance
            var summary = await _accountSummaryRepos.GetBy(tranx.AccountNumber);
            Console.WriteLine(tranx.TransactionType.ToString());
            if (tranx.TransactionType.Equals(TransactionType.Withdraw))
            {
                if (summary.Balance < tranx.Amount)
                {
                    throw new InsufficientBalanceException("Insufficient balance");
                }
                summary.Balance -= tranx.Amount;
            }
            else
            {
                summary.Balance += tranx.Amount;
            }
            Console.WriteLine("b4 save: "+summary.Balance);
            //ensure data is saved, 
            var isSaved = false;
            while (!isSaved)
            {
                try
                {
                    await _accountSummaryRepos.Update(summary);
                    isSaved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is AccountSummary)
                        {
                            var databaseValues = entry.GetDatabaseValues();

                            if (databaseValues != null)
                            {
                                entry.OriginalValues.SetValues(databaseValues);
                                CalculateNewBalance();

                                void CalculateNewBalance()
                                {
                                    var balance = (decimal)entry.OriginalValues["Balance"];
                                    var amount = tranx.Amount;

                                    if (tranx.TransactionType == TransactionType.Deposit)
                                    {
                                        summary.Balance = balance += amount;
                                    }
                                    else if (tranx.TransactionType == TransactionType.Withdraw)
                                    {
                                        if (amount > balance)
                                            throw new InsufficientBalanceException("Insufficient balance");

                                        summary.Balance = balance -= amount;
                                    }
                                }
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }
                        }
                    }
                }   
            }
            var newTranx = await _accountTransactionRepos.Add(tranx);
            var tranxDto = _mapper.Map<AccountTransactionDto>(newTranx);
            Console.WriteLine(tranxDto.AccountNumber +"|||"+tranxDto.Amount+"|||"+summary.Balance);
            var tranxRes = new AccountTransactionResponse(null, summary.Balance, tranxDto);
            return tranxRes;
        }

    }
}
