﻿using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile(ILocalizationService localization)
        {
            CreateMap<ResourceWithLatestVersionContract, ResourceViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceVersionId, opt => opt.MapFrom(src => src.LatestVersion.Id))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.LatestVersion.Author))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.LatestVersion.Comment))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.LatestVersion.CreateDate))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.LatestVersion.VersionNumber))
                .ForMember(dest => dest.RelatedResourceName, opt => opt.MapFrom(src => src.LatestVersion.RelatedResource.Name))
                .ForMember(dest => dest.IsSelected, opt => opt.Ignore());
            
            CreateMap<ResourceVersionContract, ResourceVersionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.CreateDateString, opt => opt.MapFrom(src => 
                    src.CreateDate.ToLocalTime().ToString(localization.GetRequestCulture())))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.VersionNumber));
        }
    }
}