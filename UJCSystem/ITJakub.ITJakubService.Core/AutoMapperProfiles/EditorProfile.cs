using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using Vokabular.Shared.DataContracts.Search.Old.SearchDetail;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class EditorProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Responsible, EditorContract>();
        }
    }
}