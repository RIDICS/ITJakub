﻿using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.ProjectType))
                .ForMember(dest => dest.TextType, opt => opt.MapFrom(src => src.TextType));
                //.ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId));

            CreateMap<Project, GetProjectContract>()
                .IncludeBase<Project, ProjectContract>()
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser));

            CreateMap<Project, ProjectDetailContract>()
                .IncludeBase<Project, ProjectContract>()
                .ForMember(dest => dest.LatestMetadata, opt => opt.Ignore())
                .ForMember(dest => dest.PageCount, opt => opt.Ignore())
                .ForMember(dest => dest.Authors, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsiblePersons, opt => opt.Ignore())
                .ForMember(dest => dest.EditedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.LatestChangeTime, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentUserPermissions, opt => opt.Ignore());


            CreateMap<ProjectTypeEnum, ProjectTypeContract>().ReverseMap();

            CreateMap<TextTypeEnum, TextTypeEnumContract>().ReverseMap();
        }
    }
}
