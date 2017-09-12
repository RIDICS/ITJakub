using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookTypeEnum, BookTypeEnumContract>().ReverseMap();

            CreateMap<MetadataResource, BookContract>()
                .IncludeBase<MetadataResource, ProjectMetadataContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Project.Id));
        }
    }
}