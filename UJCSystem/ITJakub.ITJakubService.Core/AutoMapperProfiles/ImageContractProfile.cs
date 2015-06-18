using AutoMapper;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class ImageContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<image, ImageContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id));
        }
    }
}