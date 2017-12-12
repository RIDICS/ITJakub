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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                //.ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId));

            CreateMap<Project, GetProjectContract>()
                .IncludeBase<Project, ProjectContract>()
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser));

            CreateMap<Project, ProjectDetailContract>()
                .IncludeBase<Project, ProjectContract>()
                .ForMember(dest => dest.Authors, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsiblePersons, opt => opt.Ignore());
        }
    }
}
