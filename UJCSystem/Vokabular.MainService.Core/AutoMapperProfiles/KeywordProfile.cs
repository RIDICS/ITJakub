using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class KeywordProfile : Profile
    {
        public KeywordProfile()
        {
            CreateMap<Keyword, KeywordContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Text));

            CreateMap<Keyword, string>()
                .ConvertUsing(x => x.Text);
        }
    }
}