using AutoMapper;
using ITJakub.CardFile.Core.DataContractEntities;
using Vokabular.MainService.DataContracts.Contracts.CardFile;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class ImageContractProfile : Profile
    {
        public ImageContractProfile()
        {
            CreateMap<image, CardImageContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id));
        }
    }
}