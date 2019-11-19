﻿using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class PageProfile : Profile
    {
        public PageProfile()
        {
            CreateMap<PageResource, PageContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Id))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position));

            CreateMap<PageResource, PageWithContextContract>()
                .IncludeBase<PageResource, PageContract>();
            
            CreateMap<PageResource, PageWithImageInfoContract>()
                .IncludeBase<PageResource, PageContract>()
                .ForMember(dest => dest.HasImage, opt => opt.Ignore());
        }
    }
}