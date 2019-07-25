using AutoMapper;
using Ridics.Authentication.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class UserContactProfile : Profile
    {
        public UserContactProfile()
        {
            CreateMap<ContactTypeEnum, ContactTypeContract>()
                .ReverseMap();
        }
    }
}
