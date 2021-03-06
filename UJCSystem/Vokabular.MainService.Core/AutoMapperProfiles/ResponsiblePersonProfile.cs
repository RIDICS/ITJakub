﻿using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ResponsiblePersonProfile : Profile
    {
        public ResponsiblePersonProfile()
        {
            CreateMap<ResponsiblePerson, ResponsiblePersonContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<ResponsibleType, ResponsibleTypeContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<ResourceTypeEnum, ResponsibleTypeEnumContract>().ReverseMap();


            CreateMap<ProjectResponsiblePerson, ProjectResponsiblePersonContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ResponsiblePerson.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ResponsiblePerson.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ResponsiblePerson.LastName))
                .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
                .ForMember(dest => dest.ResponsibleType, opt => opt.MapFrom(src => src.ResponsibleType));
        }
    }
}