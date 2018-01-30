using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class LiteraryKindProfile : Profile
    {
        public LiteraryKindProfile()
        {
            CreateMap<LiteraryKind, LiteraryKindContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<LiteraryKind, string>()
                .ConvertUsing(x => x.Name);
        }
    }
}