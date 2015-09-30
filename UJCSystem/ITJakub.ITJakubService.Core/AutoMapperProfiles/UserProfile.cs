using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<User, UserContract>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(m => m.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(m => m.CreateTime, opt => opt.MapFrom(src => src.CreateTime));
        }
    }
}