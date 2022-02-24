using System;
using System.Threading.Tasks;
using Transaction.API.DTOs;
using Transaction.API.Models;

namespace Transaction.API.Services
{
    public interface ITransactionService
    {
        Task<AccountTransactionResponse> ExecuteTransaction (AccountTransaction tranx);
    }
}
