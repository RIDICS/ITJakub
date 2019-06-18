using AutoMapper;
using ITJakub.Web.Hub.Models;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class AccountDetailProfile : Profile
    {
        public AccountDetailProfile()
        {
            CreateMap<UserDetailContract, AccountDetailViewModel>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
