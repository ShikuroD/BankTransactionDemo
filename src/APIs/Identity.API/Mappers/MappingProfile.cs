using System;
using AutoMapper;
using Identity.API.DTOs;
using Identity.API.Models;

namespace Identity.API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
