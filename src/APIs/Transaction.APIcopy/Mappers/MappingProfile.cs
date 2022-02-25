using System;
using AutoMapper;
using Transaction.APIcopy.DTOs;
using Transaction.APIcopy.Models;

namespace Transaction.APIcopy.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountSummary, AccountSummaryDto>();
            CreateMap<AccountSummaryDto, AccountSummary>();

            CreateMap<AccountTransaction, AccountTransactionDto>();
            CreateMap<AccountTransactionDto, AccountTransaction>();
        }
    }
}
