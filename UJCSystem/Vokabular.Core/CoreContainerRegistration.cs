using Microsoft.Extensions.DependencyInjection;
using Vokabular.Core.Search;
using Vokabular.Core.Storage;
using Vokabular.Core.Storage.PathResolvers;
using Vokabular.Shared.Container;

namespace Vokabular.Core
{
    public class CoreContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<MetadataSearchCriteriaDirector>();
            services.AddScoped<MetadataSearchCriteriaProcessor>();

            services.AddScoped<FileSystemManager>();
            services.AddScoped<IResourceTypePathResolver, ConvertedMetadataPathResolver>();
            services.AddScoped<IResourceTypePathResolver, UploadedMetaDataPathResolver>();
            services.AddScoped<IResourceTypePathResolver, BookPathResolver>();
            services.AddScoped<IResourceTypePathResolver, PagePathResolver>();
            services.AddScoped<IResourceTypePathResolver, SourceDocumentPathResolver>();
            services.AddScoped<IResourceTypePathResolver, TransformationPathResolver>();
            services.AddScoped<IResourceTypePathResolver, ImagePathResolver>();
            services.AddScoped<IResourceTypePathResolver, AudioPathResolver>();
        }
    }
}
