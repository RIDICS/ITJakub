﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Extensions;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceProjectClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceProjectClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public PagedResultList<ProjectDetailContract> GetProjectList(int start, int count, ProjectTypeContract projectType,
            ProjectOwnerTypeContract projectOwnerType = ProjectOwnerTypeContract.AllProjects, string filterByName = null, bool fetchPageCount = false,
            bool fetchLatestChangedResource = false, bool fetchPermissions = false)
        {
            try
            {
                var url = UrlQueryBuilder.Create("project")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("projectType", projectType)
                    .AddParameter("projectOwnerType", projectOwnerType)
                    .AddParameter("filterByName", filterByName)
                    .AddParameter("fetchPageCount", fetchPageCount)
                    .AddParameter("fetchLatestChangedResource", fetchLatestChangedResource)
                    .AddParameter("fetchPermissions", fetchPermissions)
                    .ToResult();
                var result = m_client.GetPagedList<ProjectDetailContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public ProjectDetailContract GetProject(long projectId, bool fetchPageCount = false, bool fetchPermissions = false)
        {
            try
            {
                var project = m_client.Get<ProjectDetailContract>($"project/{projectId}?fetchPageCount={fetchPageCount}&fetchPermissions={fetchPermissions}");
                return project;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateProject(CreateProjectContract project)
        {
            try
            {
                var projectId = m_client.Post<long>("project", project);
                return projectId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveProject(long projectId)
        {
            try
            {
                m_client.Delete($"project/{projectId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public void UpdateProject(long projectId, ItemNameContract data)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public ProjectMetadataResultContract GetProjectMetadata(long projectId, bool includeAuthor,
            bool includeResponsiblePerson, bool includeKind, bool includeGenre, bool includeOriginal, bool includeKeyword,
            bool includeCategory)
        {
            try
            {
                var metadata =
                    m_client.Get<ProjectMetadataResultContract>(
                        $"project/{projectId}/metadata?includeAuthor={includeAuthor}&includeResponsiblePerson={includeResponsiblePerson}&includeKind={includeKind}&includeGenre={includeGenre}&includeOriginal={includeOriginal}&includeKeyword={includeKeyword}&includeCategory={includeCategory}");
                return metadata;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<string> GetPublisherAutoComplete(string query)
        {
            try
            {
                var publishers = m_client.Get<List<string>>($"metadata/publisher/autocomplete?query={query}");
                return publishers;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateNewProjectMetadataVersion(long projectId, ProjectMetadataContract metadata)
        {
            try
            {
                var newResourceVersion = m_client.Post<long>($"project/{projectId}/metadata", metadata);
                return newResourceVersion;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetProjectLiteraryKinds(long projectId, IntegerIdListContract request)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/literary-kind", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetProjectCategories(long projectId, IntegerIdListContract request)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/category", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetProjectKeywords(long projectId, IntegerIdListContract request)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/keyword", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetProjectLiteraryGenres(long projectId, IntegerIdListContract request)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/literary-genre", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetProjectAuthors(long projectId, IntegerIdListContract request)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/author", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetProjectResponsiblePersons(long projectId, List<ProjectResponsiblePersonIdContract> request)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/responsible-person", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ResourceWithLatestVersionContract> GetResourceList(long projectId, ResourceTypeEnumContract? resourceType = null)
        {
            try
            {
                var url = $"project/{projectId}/resource";
                if (resourceType != null)
                {
                    url = url.AddQueryString("resourceType", resourceType.ToString());
                }

                var result = m_client.Get<List<ResourceWithLatestVersionContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveResource(long resourceId)
        {
            try
            {
                m_client.Delete($"project/resource/{resourceId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<ResourceVersionContract> GetResourceVersionHistory(long resourceId, int? higherVersion, int lowerVersion)
        {
            try
            {
                var url = UrlQueryBuilder.Create($"project/resource/{resourceId}/version")
                    .AddParameter("higherVersion", higherVersion)
                    .AddParameter("lowerVersion", lowerVersion)
                    .ToResult();
                var result = m_client.Get<IList<ResourceVersionContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<SnapshotAggregatedInfoContract> GetSnapshotList(long projectId, int start, int count, string query)
        {
            try
            {
                var url = UrlQueryBuilder.Create($"project/{projectId}/snapshot")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByComment", query)
                    .ToResult();

                var result = m_client.GetPagedList<SnapshotAggregatedInfoContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PageContract> GetAllPageList(long projectId)
        {
            try
            {
                var result = m_client.Get<List<PageContract>>($"project/{projectId}/page");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public List<PageWithImageInfoContract> GetAllPagesWithImageInfoList(long projectId)
        {
            try
            {
                var result = m_client.Get<List<PageWithImageInfoContract>>($"project/{projectId}/page-image-info");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        public List<TextWithPageContract> GetAllTextResourceList(long projectId, long? resourceGroupId)
        {
            try
            {
                var result =
                    m_client.Get<List<TextWithPageContract>>($"project/{projectId}/text?resourceGroupId={resourceGroupId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FullTextContract GetTextResourceByPageId(long pageId, TextFormatEnumContract? format)
        {
            try
            {
                var result = m_client.Get<FullTextContract>($"project/page/{pageId}/text?format={format.ToString()}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public ImageContract GetImageResourceByPageId(long pageId)
        {
            try
            {
                var result = m_client.Get<ImageContract>($"project/page/{pageId}/image");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FullTextContract GetTextResource(long textId, TextFormatEnumContract? format)
        {
            try
            {
                var result = m_client.Get<FullTextContract>($"project/text/{textId}?format={format.ToString()}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FullTextContract GetTextResourceVersion(long textVersionId, TextFormatEnumContract? format)
        {
            try
            {
                var result = m_client.Get<FullTextContract>($"project/text/version/{textVersionId}?format={format.ToString()}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<GetTextCommentContract> GetCommentsForText(long textId)
        {
            try
            {
                var result = m_client.Get<List<GetTextCommentContract>>($"project/text/{textId}/comment");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public GetTextCommentContract GetComment(long commentId)
        {
            try
            {
                var result = m_client.Get<GetTextCommentContract>($"project/text/comment/{commentId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateComment(long textId, CreateTextCommentContract request)
        {
            try
            {
                var result = m_client.Post<long>($"project/text/{textId}/comment", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateComment(long commentId, CreateTextCommentContract request)
        {
            try
            {
                m_client.Put<object>($"project/text/comment/{commentId}", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteComment(long commentId)
        {
            try
            {
                m_client.Delete($"project/text/comment/{commentId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public long CreatePage(long projectId, CreatePageContract request)
        {
            try
            {
                var result = m_client.Post<long>($"project/{projectId}/page", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public long CreateTextResource(long pageId, CreateTextRequestContract request)
        {
            try
            {
                var result = m_client.Post<long>($"project/page/{pageId}/text", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateTextResourceVersion(long textId, CreateTextVersionRequestContract request)
        {
            try
            {
                var result = m_client.Post<long>($"project/text/{textId}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public NewResourceResultContract CreateImageResource(CreateImageContract data, Stream dataStream)
        {
            try
            {
                var formData = FormDataBuilder.Create()
                    .AddParameter(nameof(CreateImageContract.ImageId), data.ImageId)
                    .AddParameter(nameof(CreateImageContract.OriginalVersionId), data.OriginalVersionId)
                    .AddParameter(nameof(CreateImageContract.ResourcePageId), data.ResourcePageId)
                    .AddParameter(nameof(CreateImageContract.Comment), data.Comment)
                    .AddParameter(nameof(CreateImageContract.FileName), data.FileName)
                    .ToResult();
                
                var result = m_client.PostStreamAsForm<NewResourceResultContract>("project/image", dataStream, data.FileName, formData);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<UserGroupContract> GetUserGroupsByProject(long projectId, int start, int count, string query)
        {
            try
            {
                var url = UrlQueryBuilder.Create($"project/{projectId}/user-group")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByName", query)
                    .ToResult();

                var result = m_client.GetPagedList<UserGroupContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateForum(long projectId)
        {
            try
            {
                var forumId = m_client.Post<int>($"project/{projectId}/forum", null);
                return forumId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public ForumContract GetForum(long projectId)
        {
            try
            {
                var result = m_client.Get<ForumContract>($"project/{projectId}/forum");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public void AddProjectToUserGroupByCode(long projectId, AssignPermissionToSingleUserGroupContract data)
        {
            try
            {
                m_client.Post<object>($"project/{projectId}/single-user-group", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetAllPageList(long projectId, IList<CreateOrUpdatePageContract> pageList)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/page", pageList);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public SnapshotContract GetLatestPublishedSnapshot(long projectId)
        {
            try
            {
                var result = m_client.Get<SnapshotContract>($"project/{projectId}/snapshot/latest");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetPageText(long pageId, TextFormatEnumContract format)
        {
            try
            {
                var result = m_client.GetString($"project/page/{pageId}/text-content?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetPageImage(long pageId)
        {
            try
            {
                var result = m_client.GetStream($"project/page/{pageId}/image-content");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public bool HasPageImage(long pageId)
        {
            try
            {
                m_client.Head($"project/page/{pageId}/image");
                return true;
            }
            catch (HttpRequestException e)
            {
                var statusException = e as HttpErrorCodeException;
                if (statusException?.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public EditionNoteContract GetLatestEditionNote(long projectId, TextFormatEnumContract format)
        {
            try
            {
                var result = m_client.Get<EditionNoteContract>($"project/{projectId}/edition-note?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateEditionNote(long projectId, CreateEditionNoteContract data)
        {
            try
            {
                return m_client.Post<long>($"project/{projectId}/edition-note", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetImageResource(long imageId)
        {
            try
            {
                var result = m_client.GetStream($"project/image/{imageId}/content");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetImageResourceVersion(long imageVersionId)
        {
            try
            {
                var result = m_client.GetStream($"project/image/version/{imageVersionId}/content");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<ImageWithPageContract> GetProjectImages(long projectId)
        {
            try
            {
                var result = m_client.Get<IList<ImageWithPageContract>>($"project/{projectId}/image");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public TrackContract GetAudioTrack(long trackId)
        {
            try
            {
                var result = m_client.Get<TrackContract>($"project/track/{trackId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<AudioContract> GetAudioTrackRecordings(long trackId)
        {
            try
            {
                var result = m_client.Get<IList<AudioContract>>($"project/track/{trackId}/recordings");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetAudio(long audioId)
        {
            try
            {
                var result = m_client.GetStream($"project/audio/{audioId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public List<TermContract> GetPageTermList(long pageId)
        {
            try
            {
                var result = m_client.Get<List<TermContract>>($"project/page/{pageId}/term");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public void SetTerms(long pageId, IntegerIdListContract termIdList)
        {
            try
            { 
                m_client.Put<object>($"project/page/{pageId}/term", termIdList);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #region Chapters

        public IList<ChapterHierarchyDetailContract> GetChapterList(long projectId)
        {
            try
            {
                var result = m_client.Get<IList<ChapterHierarchyDetailContract>>($"project/{projectId}/chapter");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public GetChapterContract GetChapter(long chapterId)
        {
            try
            {
                var result = m_client.Get<GetChapterContract>($"project/chapter/{chapterId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateChapter(long projectId, CreateChapterContract request)
        {
            try
            {
                var result = m_client.Post<long>($"project/{projectId}/chapter", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateChapter(long chapterId, CreateChapterContract request)
        {
            try
            {
                m_client.Put<object>($"project/chapter/{chapterId}", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateChapterList(long projectId, IList<CreateOrUpdateChapterContract> request)
        {
            try
            {
                m_client.Put<object>($"project/{projectId}/chapter", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void GenerateChapters(long projectId)
        {
            try
            {
                m_client.Post<object>($"project/{projectId}/chapter/generator", null);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Project groups

        public ProjectGroupContract GetProjectGroups(long projectId)
        {
            try
            {
                var result = m_client.Get<ProjectGroupContract>($"project/{projectId}/group");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddProjectToGroup(long targetProjectId, long selectedProjectId)
        {
            try
            {
                m_client.Put<object>($"project/{targetProjectId}/group?selectedProjectId={selectedProjectId}", null);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveProjectFromGroup(long projectId)
        {
            try
            {
                m_client.Delete($"project/{projectId}/group");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PermissionDataContract GetCurrentUserProjectPermissions(long projectId)
        {
            try
            {
                var result = m_client.Get<PermissionDataContract>($"project/{projectId}/current-user-permission");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

    }
}
