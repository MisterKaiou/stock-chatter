using AutoMapper;
using StockChatter.API.Domain.Entitites.Users;
using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDAO>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
        }
    }
}
