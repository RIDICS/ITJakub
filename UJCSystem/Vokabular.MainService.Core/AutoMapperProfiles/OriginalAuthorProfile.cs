using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class OriginalAuthorProfile : Profile
    {
        public OriginalAuthorProfile()
        {
            CreateMap<OriginalAuthor, OriginalAuthorContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<ProjectOriginalAuthor, OriginalAuthorContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OriginalAuthor.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.OriginalAuthor.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.OriginalAuthor.LastName));
        }
    }
}