using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;
using Vokabular.Shared.DataContracts.Search.Old.SearchDetail;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class PublisherProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Publisher, PublisherContract>();
        }
    }
}