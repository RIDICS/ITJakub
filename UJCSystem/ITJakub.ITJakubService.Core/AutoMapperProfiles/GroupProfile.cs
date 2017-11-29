using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupDetailContract>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(m => m.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(m => m.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(m => m.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(m => m.Members, opt => opt.MapFrom(src => src.Users));

            CreateMap<Group, GroupContract>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(m => m.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}