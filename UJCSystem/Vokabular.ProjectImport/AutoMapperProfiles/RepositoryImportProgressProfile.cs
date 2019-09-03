using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;
using Vokabular.ProjectImport.Model;

namespace Vokabular.ProjectImport.AutoMapperProfiles
{
    public class RepositoryImportProgressProfile : Profile
    {
        public RepositoryImportProgressProfile()
        {
            CreateMap<RepositoryImportProgressInfo, RepositoryImportProgressInfoContract> ()
                .ForMember(dest => dest.ExternalRepositoryId, opt => opt.MapFrom(src => src.ExternalRepositoryId))
                .ForMember(dest => dest.ExternalRepositoryName, opt => opt.MapFrom(src => src.ExternalRepositoryName))
                .ForMember(dest => dest.FailedProjectsCount, opt => opt.MapFrom(src => src.FailedProjectsCount))
                .ForMember(dest => dest.FaultedMessage, opt => opt.MapFrom(src => src.FaultedMessage))
                .ForMember(dest => dest.FaultedMessageParams, opt => opt.MapFrom(src => src.FaultedMessageParams))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
                .ForMember(dest => dest.ProcessedProjectsCount, opt => opt.MapFrom(src => src.ProcessedProjectsCount))
                .ForMember(dest => dest.TotalProjectsCount, opt => opt.MapFrom(src => src.TotalProjectsCount));
        }
    }
}