using AutoMapper;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ForumProfile : Profile
    {
        public ForumProfile()
        {
            CreateMap<Forum, ForumContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.ForumID))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name));
        }
    }
}