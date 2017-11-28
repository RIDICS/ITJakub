using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.Text;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class PageManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly UserManager m_userManager;
        private readonly FileSystemManager m_fileSystemManager;

        public PageManager(ResourceRepository resourceRepository, UserManager userManager, FileSystemManager fileSystemManager)
        {
            m_resourceRepository = resourceRepository;
            m_userManager = userManager;
            m_fileSystemManager = fileSystemManager;
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

        public List<ImageWithPageContract> GetImageResourceList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectImages(projectId, null, true));
            var sortedDbResult = dbResult.OrderBy(x => ((PageResource)x.ResourcePage.LatestVersion).Position);
            var result = Mapper.Map<List<ImageWithPageContract>>(sortedDbResult);
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
                    result.Text = "**One morning**, when *Gregor Samsa* woke from troubled dreams, he found himself transformed in his bed into a horrible vermin. He lay on his armour-like back, and if he lifted his head a little he could see his brown belly, slightly domed and divided by arches into stiff sections. The bedding was hardly able to cover it and seemed ready to slide off any moment. His many legs, pitifully thin compared with the size of the rest of him, waved about helplessly as he looked. \"What's happened to me? \" he thought. It wasn't a dream.His room, a proper human room although a little too small, lay peacefully between its four familiar walls.";
                    break;
                case TextFormatEnumContract.Html:
                    result.Text = "<b>One morning</b>, when <i>Gregor Samsa</i> woke from <span id=\"51c8716b-7091-4ab3-a9f6-73546e46e53f-text\">troubled</span> dreams, he found himself transformed in his bed into a horrible vermin. He lay on his armour-like back, and if he lifted his head a little he could see his brown belly, slightly domed and divided by arches into stiff sections. The bedding was hardly able to cover it and seemed ready to slide off any moment. His many legs, pitifully thin compared with the size of the rest of him, waved about helplessly as he looked. \"What's happened to me? \" he thought. It wasn't a dream.His room, a proper human room although a little too small, lay peacefully between its four familiar walls.";
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

        public FileResultData GetImageResource(long imageId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<ImageResource>(imageId));

            var imageStream = m_fileSystemManager.GetResource(dbResult.Resource.Project.Id, null, dbResult.FileId, ResourceType.Image);
            return new FileResultData
            {
                FileName = dbResult.FileName,
                MimeType = dbResult.MimeType,
                Stream = imageStream,
                FileSize = imageStream.Length,
            };
        }
    }
}