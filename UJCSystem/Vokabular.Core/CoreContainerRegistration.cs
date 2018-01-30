using Vokabular.Core.Search;
using Vokabular.Core.Storage;
using Vokabular.Core.Storage.PathResolvers;
using Vokabular.Shared.Container;

namespace Vokabular.Core
{
    public class CoreContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<MetadataSearchCriteriaDirector>();
            container.AddPerWebRequest<MetadataSearchCriteriaProcessor>();

            container.AddPerWebRequest<FileSystemManager>();
            container.AddPerWebRequest<IResourceTypePathResolver, ConvertedMetadataPathResolver>();
            container.AddPerWebRequest<IResourceTypePathResolver, UploadedMetaDataPathResolver>();
            container.AddPerWebRequest<IResourceTypePathResolver, BookPathResolver>();
            container.AddPerWebRequest<IResourceTypePathResolver, PagePathResolver>();
            container.AddPerWebRequest<IResourceTypePathResolver, SourceDocumentPathResolver>();
            container.AddPerWebRequest<IResourceTypePathResolver, TransformationPathResolver>();
            container.AddPerWebRequest<IResourceTypePathResolver, ImagePathResolver>();
            container.AddPerWebRequest<IResourceTypePathResolver, AudioPathResolver>();
        }
    }
}
