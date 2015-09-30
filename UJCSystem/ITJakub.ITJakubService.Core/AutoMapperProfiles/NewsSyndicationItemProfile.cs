using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts.News;
using MobileContracts = ITJakub.MobileApps.MobileContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class NewsSyndicationItemProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<NewsSyndicationItem, NewsSyndicationItemContract>()
                .ForMember(m => m.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(m => m.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(m => m.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(m => m.ItemType, opt => opt.MapFrom(src => src.ItemType))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(m => m.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(m => m.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(m => m.UserLastName, opt => opt.MapFrom(src => src.User.LastName));

            CreateMap<NewsSyndicationItem, MobileContracts.News.NewsSyndicationItemContract>()
                .ForMember(m => m.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(m => m.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(m => m.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(m => m.ItemType, opt => opt.MapFrom(src => src.ItemType))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(m => m.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(m => m.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(m => m.UserLastName, opt => opt.MapFrom(src => src.User.LastName));

        }


    }
}