using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Works.Text;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.Managers
{
    public class PageManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly UserManager m_userManager;
        private readonly CommunicationProvider m_communicationProvider;

        public PageManager(ResourceRepository resourceRepository, UserManager userManager, CommunicationProvider communicationProvider)
        {
            m_resourceRepository = resourceRepository;
            m_userManager = userManager;
            m_communicationProvider = communicationProvider;
        }

        public List<PageContract> GetPageList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectPages(projectId));
            var result = Mapper.Map<List<PageContract>>(dbResult);
            return result;
        }

        public List<TextWithPageContract> GetTextResourceList(long projectId, long? resourceGroupId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectTexts(projectId, resourceGroupId, true));
            var sortedDbResult = dbResult.OrderBy(x => ((PageResource) x.ParentResource.LatestVersion).Position);
            var result = Mapper.Map<List<TextWithPageContract>>(sortedDbResult);
            return result;
        }

        public FullTextContract GetTextResource(long textId, TextFormatEnumContract formatValue)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetTextResource(textId));
            var result = Mapper.Map<FullTextContract>(dbResult);
            
            var client = m_communicationProvider.GetFulltextServiceClient();
            
            var textResource = client.GetTextResource(result.ExternalId, (int)formatValue);
            result.Text = textResource.Text;

            return result;
        }

        public List<GetTextCommentContract> GetCommentsForText(long textId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetCommentsForText(textId));
            var result = Mapper.Map<List<GetTextCommentContract>>(dbResult);
            return result;
        }
        
        public long CreateNewComment(long textId, CreateTextCommentContract newComment)
        {
            var userId = m_userManager.GetCurrentUserId();
            var createNewCommentWork = new CreateNewTextCommentWork(m_resourceRepository, textId, newComment, userId);
            var resultId = createNewCommentWork.Execute();
            return resultId;
        }

        public void DeleteComment(long commentId)
        {
            var deleteCommentWork = new DeleteTextCommentWork(m_resourceRepository, commentId);
            deleteCommentWork.Execute();
        }

        public long CreateNewTextResourceVersion(ShortTextContract request)
        {
            var userId = m_userManager.GetCurrentUserId();
            var createNewTextResourceWork = new CreateNewTextResourceWork(m_resourceRepository, request, userId, m_communicationProvider);
            var resultId = createNewTextResourceWork.Execute();
            return resultId;
        }
    }
}