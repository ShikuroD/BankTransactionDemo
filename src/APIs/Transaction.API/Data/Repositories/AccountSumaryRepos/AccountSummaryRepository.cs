using System;
using Transaction.API.Models;
using AutoMapper;
namespace Transaction.API.Data.Repositories
{
    public class AccountSummaryRepository : Repository<AccountSummary>, IAccountSummaryRepository
    {
        private readonly TransactionContext _context;
        private readonly IMapper _mapper;

        public AccountSummaryRepository(TransactionContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
        
    }
}
