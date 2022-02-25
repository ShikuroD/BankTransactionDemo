using System;
using System.Threading.Tasks;
using Transaction.APIcopy.DTOs;
using Transaction.APIcopy.Models;

namespace Transaction.APIcopy.Services
{
    public interface ITransactionService
    {
        Task<AccountTransactionResponse> ExecuteTransaction (AccountTransaction tranx);
    }
}
