﻿using System.Collections.Generic;
using AutoMapper;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class TokenDistanceListCriteriaDescriptionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<TokenDistanceListCriteriaDescription, TokenDistanceListCriteriaContract>()
                .ForMember(dest => dest.Disjunctions, opt => opt.MapFrom(src => Mapper.Map<IList<TokenDistanceCriteriaContract>>(src.Disjunctions)))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => (CriteriaKey) src.SearchType))
                .ForAllMembers(opt => opt.Condition(src => !src.IsSourceValueNull));
        }
    }
}