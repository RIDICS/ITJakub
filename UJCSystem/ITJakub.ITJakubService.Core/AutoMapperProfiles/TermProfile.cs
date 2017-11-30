using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class TermProfile : Profile
    {
        public TermProfile()
        {
            CreateMap<Term, TermContract>()
                .ForMember(dest => dest.XmlId, opts => opts.MapFrom(src => src.XmlId))
                .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.Text))
                .ForMember(dest => dest.Position, opts => opts.MapFrom(src => src.Position));
        }
    }
}