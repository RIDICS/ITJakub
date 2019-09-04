using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;

namespace Vokabular.ProjectImport.AutoMapperProfiles
{
    public class ExternalRepositoryTypeProfile : Profile
    {
        public ExternalRepositoryTypeProfile()
        {
            CreateMap<ExternalRepositoryType, ExternalRepositoryTypeContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}