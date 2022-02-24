using System;
using AutoMapper;
using Transaction.API.DTOs;
using Transaction.API.Models;

namespace Transaction.API.Mappers
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
