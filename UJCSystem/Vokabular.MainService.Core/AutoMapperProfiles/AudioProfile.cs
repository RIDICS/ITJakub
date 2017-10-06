using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class AudioProfile : Profile
    {
        public AudioProfile()
        {
            CreateMap<AudioTypeEnum, AudioTypeEnumContract>().ReverseMap();

            CreateMap<AudioResource, AudioContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Id))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.VersionNumber))
                .ForMember(dest => dest.AudioType, opt => opt.MapFrom(src => src.AudioType))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType));
        }
    }
}