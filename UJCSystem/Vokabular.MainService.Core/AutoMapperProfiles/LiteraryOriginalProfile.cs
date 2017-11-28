using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class LiteraryOriginalProfile : Profile
    {
        public LiteraryOriginalProfile()
        {
            CreateMap<LiteraryOriginal, LiteraryOriginalContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<LiteraryOriginal, string>()
                .ConvertUsing(x => x.Name);
        }
    }
}