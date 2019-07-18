using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.OaiPmhImportManager.Model;

namespace Vokabular.OaiPmhImportManager.AutoMapperProfiles
{
    public class OaiPmhRepositoryInfoProfile : Profile
    {
        public OaiPmhRepositoryInfoProfile()
        {
            CreateMap<OaiPmhRepositoryInfo, OaiPmhRepositoryInfoContract> ()
                .ForMember(dest => dest.AdminMails, opt => opt.MapFrom(src => src.AdminMails))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.EarliestDateTime, opt => opt.MapFrom(src => src.EarliestDateTime))
                .ForMember(dest => dest.Granularity, opt => opt.MapFrom(src => src.Granularity.ToString()))
                .ForMember(dest => dest.MetadataFormats, opt => opt.MapFrom(src => src.MetadataFormatTypes))
                .ForMember(dest => dest.Sets, opt => opt.MapFrom(src => src.SetTypes));
        }
    }
}