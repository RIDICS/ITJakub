using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.OaiPmhImportManager.Model;

namespace Vokabular.OaiPmhImportManager.AutoMapperProfiles
{
    public class OaiPmhRepositoryConfigurationProfile : Profile
    {
        public OaiPmhRepositoryConfigurationProfile()
        {
            CreateMap<OaiPmhRepositoryConfiguration, OaiPmhRepositoryConfigurationContract> ()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.DataFormat, opt => opt.MapFrom(src => src.DataFormat))
                .ForMember(dest => dest.SetName, opt => opt.MapFrom(src => src.SetName));
        }
    }
}