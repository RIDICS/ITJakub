using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class ForumProfile : Profile
    {
        public ForumProfile()
        {
            CreateMap<ForumContract, ForumViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));
        }
    }
}