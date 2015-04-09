using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BookPageProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookPage, BookPageContract>();

            CreateMap<BookPageContract, MobileApps.MobileContracts.PageContract>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Text));
        }
    }
}