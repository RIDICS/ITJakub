using AutoMapper;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.OaiPmhImportManager.Model;

namespace Vokabular.OaiPmhImportManager.AutoMapperProfiles
{
    public class SetProfile : Profile
    {
        public SetProfile()
        {
            CreateMap<setType, SetContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.setSpec))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.setName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.setDescription.ToString()));
        }
    }
}