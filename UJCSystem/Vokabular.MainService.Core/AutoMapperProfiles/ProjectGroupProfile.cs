﻿using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ProjectGroupProfile : Profile
    {
        public ProjectGroupProfile()
        {
            CreateMap<ProjectGroup, ProjectGroupContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Projects, opt => opt.MapFrom(src => src.Projects))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime));
        }
    }
}