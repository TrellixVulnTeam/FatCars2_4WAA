using AutoMapper;
using FatCars.Application.Dtos;
using FatCars.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FatCars.Application.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Users, UserDto>();
            CreateMap<Email, EmailDto>().ReverseMap();
        }
    }
}
