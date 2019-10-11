using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;

namespace Vokabular.ProjectImport.AutoMapperProfiles
{
    public class ExternalRepositoryProfile : Profile
    {
        public ExternalRepositoryProfile()
        {
            CreateMap<ExternalRepository, ExternalRepositoryContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.BibliographicFormat, opt => opt.MapFrom(src => src.BibliographicFormat))
                .ForMember(dest => dest.ExternalRepositoryType, opt => opt.MapFrom(src => src.ExternalRepositoryType));

            CreateMap<ExternalRepository, ExternalRepositoryDetailContract>()
                .IncludeBase<ExternalRepository, ExternalRepositoryContract>()
                .ForMember(dest => dest.Configuration, opt => opt.MapFrom(src => src.Configuration))
                .ForMember(dest => dest.License, opt => opt.MapFrom(src => src.License))
                .ForMember(dest => dest.UrlTemplate, opt => opt.MapFrom(src => src.UrlTemplate))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));
        }
    }
}