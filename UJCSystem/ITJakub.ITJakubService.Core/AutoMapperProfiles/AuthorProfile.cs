using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class AuthorProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<AuthorDetailContract, Author>().ReverseMap();
        }
    }
}