using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using Vokabular.Shared.DataContracts.Search.Old.SearchDetail;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorContract>();
            CreateMap<Author, MobileApps.MobileContracts.AuthorContract>();
        }
    }
}