using System;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreateUser, opt => opt.MapFrom(src => src.CreatedByUser))
                .ForMember(dest => dest.LastEditDate, opt => opt.UseValue(new DateTime()))
                //.ForMember(dest => dest.LastEditUser, opt => opt.UseValue(null))
                .ForMember(dest => dest.LiteraryOriginalText, opt => opt.UseValue("NULL"))
                .ForMember(dest => dest.PageCount, opt => opt.UseValue(-1))
                .ForMember(dest => dest.PublisherText, opt => opt.UseValue("NULL"));
        }
    }
}
