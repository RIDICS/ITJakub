using AutoMapper;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class ImageContractProfile : Profile
    {
        public ImageContractProfile()
        {
            CreateMap<image, ImageContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id));
        }
    }
}