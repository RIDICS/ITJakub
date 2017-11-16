using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.Text;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.Managers
{
    public class PageManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly UserManager m_userManager;

        public PageManager(ResourceRepository resourceRepository, UserManager userManager)
        {
            m_resourceRepository = resourceRepository;
            m_userManager = userManager;
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
            var sortedDbResult = dbResult.OrderBy(x => ((PageResource) x.ResourcePage.LatestVersion).Position);
            var result = Mapper.Map<List<TextWithPageContract>>(sortedDbResult);
            return result;
        }

        public FullTextContract GetTextResource(long textId, TextFormatEnumContract formatValue)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetTextResource(textId));
            var result = Mapper.Map<FullTextContract>(dbResult);
            // TODO load data from external database

            // TODO mock:
            switch (formatValue)
            {
                case TextFormatEnumContract.Raw:
                    result.Text = "*Mock text* from **MainService.**";
                    break;
                case TextFormatEnumContract.Html:
                    result.Text = "<b>Mock <span id=\"dc3b9720-199c-4d79-93e5-ba94efb1f44a-text\">text</span></b> from <i>MainService.</i>";
                    break;
                case TextFormatEnumContract.Rtf:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(formatValue), formatValue, null);
            }
            
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
    }
}