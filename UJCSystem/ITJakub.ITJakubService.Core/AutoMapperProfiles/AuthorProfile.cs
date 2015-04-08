using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class AuthorProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Author, AuthorContract>();
            CreateMap<Author, MobileApps.MobileContracts.AuthorContract>();
        }
    }
}