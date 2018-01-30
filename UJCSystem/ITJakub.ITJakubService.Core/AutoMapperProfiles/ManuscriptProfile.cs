using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using Vokabular.Shared.DataContracts.Search.Old.SearchDetail;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class ManuscriptProfile : Profile
    {
        public ManuscriptProfile()
        {
            CreateMap<ManuscriptDescription, ManuscriptContract>();
        }
    }
}