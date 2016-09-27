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
                .ForMember(m => m.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .Include<User, UserDetailContract>()
                .Include<User, PrivateUserContract>();

            CreateMap<User, UserDetailContract>()            
                .ForMember(m => m.Groups, opt => opt.MapFrom(src => src.Groups));

            CreateMap<User, PrivateUserContract>()
                .ForMember(m => m.CommunicationToken, opt => opt.MapFrom(src => src.CommunicationToken))
                .ForMember(m => m.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));
        }
    }
}