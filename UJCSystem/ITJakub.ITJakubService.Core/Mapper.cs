using AutoMapper;

namespace ITJakub.ITJakubService.Core
{
    /// <summary>
    /// Static API for AutoMapper (library refactoring is unnecessary because the whole service will be removed)
    /// </summary>
    public static class Mapper
    {
        public static IMapper MapperInstance { get; set; }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return MapperInstance.Map<TSource, TDestination>(source);
        }

        public static TDestination Map<TDestination>(object source)
        {
            return MapperInstance.Map<TDestination>(source);
        }
    }
}