using System;
using Transaction.API.Models;
using AutoMapper;
namespace Transaction.API.Data.Repositories
{
    public class AccountTransactionRepository :  Repository<AccountTransaction>, IAccountTransactionRepository
    {
        private readonly TransactionContext _context;
        private readonly IMapper _mapper;

        public AccountTransactionRepository(TransactionContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        
    }
}
