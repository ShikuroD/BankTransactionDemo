using System;
using Transaction.APIcopy.Models;
using AutoMapper;
namespace Transaction.APIcopy.Data.Repositories
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
