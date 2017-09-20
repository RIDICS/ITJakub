using AutoMapper;
using Vokabular.DataEntities.Database.Entities;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class KeywordProfile : Profile
    {
        public KeywordProfile()
        {
            CreateMap<Keyword, string>()
                .ConvertUsing(x => x.Text);
        }
    }
}