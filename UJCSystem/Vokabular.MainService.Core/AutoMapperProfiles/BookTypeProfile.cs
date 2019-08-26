using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class BookTypeProfile : Profile
    {
        public BookTypeProfile()
        {
            CreateMap<BookType, BookTypeContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<BookTypeEnum, BookTypeEnumContract>().ReverseMap();
        }
    }
}