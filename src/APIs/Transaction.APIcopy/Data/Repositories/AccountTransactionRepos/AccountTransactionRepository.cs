using System;
using Transaction.APIcopy.Models;
using AutoMapper;
namespace Transaction.APIcopy.Data.Repositories
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
