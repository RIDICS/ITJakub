using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.ProjectItem;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectItemManager
    {
        private readonly UserManager m_userManager;
        private readonly ResourceRepository m_resourceRepository;
        private readonly BookRepository m_bookRepository;

        public ProjectItemManager(UserManager userManager, ResourceRepository resourceRepository, BookRepository bookRepository)
        {
            m_userManager = userManager;
            m_resourceRepository = resourceRepository;
            m_bookRepository = bookRepository;
        }

        public List<PageContract> GetPageList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectPages(projectId));
            var result = Mapper.Map<List<PageContract>>(dbResult);
            return result;
        }

        public PageContract GetPage(long pageId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<PageResource>(pageId));
            var result = Mapper.Map<PageContract>(dbResult);
            return result;
        }

        public long CreatePage(long projectId, CreatePageContract pageData)
        {
            var userId = m_userManager.GetCurrentUserId();
            var work = new CreateOrUpdatePageWork(m_resourceRepository, pageData, projectId, null, userId);
            work.Execute();

            return work.ResourceId;
        }

        public void UpdatePage(long pageId, CreatePageContract pageData)
        {
            var userId = m_userManager.GetCurrentUserId();
            var work = new CreateOrUpdatePageWork(m_resourceRepository, pageData, null, pageId, userId);
            work.Execute();
        }

        public List<TermContract> GetPageTermList(long pageId)
        {
            var dbResult = m_bookRepository.InvokeUnitOfWork(x => x.GetPageTermList(pageId));
            var result = Mapper.Map<List<TermContract>>(dbResult);
            return result;
        }

        public void SetPageTerms(long pageId, IList<int> termIdList)
        {
            new SetPageTermsWork(m_resourceRepository, pageId, termIdList).Execute();
        }

        public List<ChapterHierarchyContract> GetChapterList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectChapters(projectId));
            var result = ChaptersHelper.ChapterToHierarchyContracts(dbResult);
            return result;
        }

        public GetChapterContract GetChapterResource(long chapterId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<ChapterResource>(chapterId));
            var result = Mapper.Map<GetChapterContract>(dbResult);
            return result;
        }

        public long CreateChapterResource(long projectId, CreateChapterContract chapterData)
        {
            var userId = m_userManager.GetCurrentUserId();
            var work = new CreateOrUpdateChapterWork(m_resourceRepository, chapterData, projectId, null, userId);
            work.Execute();

            return work.ResourceId;
        }

        public void UpdateChapterResource(long chapterId, CreateChapterContract chapterData)
        {
            var userId = m_userManager.GetCurrentUserId();
            new CreateOrUpdateChapterWork(m_resourceRepository, chapterData, null, chapterId, userId).Execute();
        }
    }
}
