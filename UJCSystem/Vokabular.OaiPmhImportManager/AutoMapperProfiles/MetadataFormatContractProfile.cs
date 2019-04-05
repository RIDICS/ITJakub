using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.OaiPmhImportManager.Model;

namespace Vokabular.OaiPmhImportManager.AutoMapperProfiles
{
    public class MetadataFormatContractProfile : Profile
    {
        public MetadataFormatContractProfile()
        {
            CreateMap<metadataFormatType, MetadataFormatContract> ()
                .ForMember(dest => dest.MetadataNamespace, opt => opt.MapFrom(src => src.metadataNamespace))
                .ForMember(dest => dest.MetadataPrefix, opt => opt.MapFrom(src => src.metadataPrefix))
                .ForMember(dest => dest.Schema, opt => opt.MapFrom(src => src.schema));
        }
    }
}