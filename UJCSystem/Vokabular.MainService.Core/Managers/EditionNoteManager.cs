using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Works.ProjectItem;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.TextConverter.Markdown;

namespace Vokabular.MainService.Core.Managers
{
    public class EditionNoteManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly IMarkdownToHtmlConverter m_markdownConverter;
        private readonly IMapper m_mapper;

        public EditionNoteManager(ResourceRepository resourceRepository, FulltextStorageProvider fulltextStorageProvider,
            AuthorizationManager authorizationManager, AuthenticationManager authenticationManager,
            IMarkdownToHtmlConverter markdownConverter, IMapper mapper)
        {
            m_resourceRepository = resourceRepository;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_authorizationManager = authorizationManager;
            m_authenticationManager = authenticationManager;
            m_markdownConverter = markdownConverter;
            m_mapper = mapper;
        }

        public EditionNoteContract GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var editionNoteResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestEditionNote(projectId));
            if (editionNoteResource == null)
                return null;

            var contract = m_mapper.Map<EditionNoteContract>(editionNoteResource);

            if (!string.IsNullOrEmpty(editionNoteResource.Text))
            {
                switch (format)
                {
                    case TextFormatEnumContract.Html:
                    {
                        contract.Text = m_markdownConverter.ConvertToHtml(editionNoteResource.Text);
                        break;
                    }
                    default:
                    {
                        contract.Text = editionNoteResource.Text;
                        break;
                    }
                }
            }
            else
            {
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(editionNoteResource.Resource.Project.ProjectType);
                contract.Text = fulltextStorage.GetEditionNote(editionNoteResource, format);
            }

            return contract;
        }

        public long CreateEditionNoteVersion(long projectId, CreateEditionNoteContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var resourceVersionId = new CreateEditionNoteVersionWork(m_resourceRepository, projectId, data, userId).Execute();
            return resourceVersionId;
        }
    }
}