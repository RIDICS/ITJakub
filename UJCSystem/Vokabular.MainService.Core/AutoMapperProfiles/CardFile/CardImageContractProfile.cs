using AutoMapper;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.MainService.DataContracts.Contracts.CardFile;

namespace Vokabular.MainService.Core.AutoMapperProfiles.CardFile
{
    public class CardImageContractProfile : Profile
    {
        public CardImageContractProfile()
        {
            CreateMap<image, CardImageContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id));
        }
    }
}