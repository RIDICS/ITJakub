using AutoMapper;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.MainService.DataContracts.Contracts.CardFile;

namespace Vokabular.MainService.Core.AutoMapperProfiles.CardFile
{
    public class CardFileContractProfile : Profile
    {
        public CardFileContractProfile()
        {
            CreateMap<file, CardFileContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.name))
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.description))
                .ForMember(dest => dest.BucketsCount, opts => opts.MapFrom(src => src.buckets))
                .ForSourceMember(source => source.fields, opt => opt.DoNotValidate());
        }
    }
}