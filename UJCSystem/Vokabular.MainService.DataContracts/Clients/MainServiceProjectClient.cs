﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient;
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

        public PagedResultList<ProjectDetailContract> GetProjectList(int start, int count, string filterByName = null, bool fetchPageCount = false)
        {
            try
            {
                var url = UrlQueryBuilder.Create("project")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByName", filterByName)
                    .AddParameter("fetchPageCount", fetchPageCount)
                    .ToQuery();
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

        public ProjectDetailContract GetProject(long projectId, bool fetchPageCount = false)
        {
            try
            {
                var project = m_client.Get<ProjectDetailContract>($"project/{projectId}?fetchPageCount={fetchPageCount}");
                return project;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateProject(ProjectContract project)
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

        public void DeleteProject(long projectId)
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

        public List<ResourceContract> GetResourceList(long projectId, ResourceTypeEnumContract? resourceType = null)
        {
            try
            {
                var url = $"project/{projectId}/resource";
                if (resourceType != null)
                {
                    url.AddQueryString("resourceType", resourceType.ToString());
                }

                var result = m_client.Get<List<ResourceContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long ProcessUploadedResources(long projectId, NewResourceContract resourceInfo)
        {
            try
            {
                var resourceId = m_client.Post<long>($"project/{projectId}/resource", resourceInfo);
                return resourceId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateSnapshot(long projectId)
        {
            try
            {
                var snapshotId = m_client.Post<long>($"project/{projectId}/snapshot", null);
                return snapshotId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<SnapshotContract> GetSnapshotList(long projectId)
        {
            try
            {
                var result = m_client.Get<List<SnapshotContract>>($"project/{projectId}/snapshot");
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
                m_client.Put<HttpStatusCode>($"project/text/comment/{commentId}", request);
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

        public long CreateTextResourceVersion(long textId, CreateTextRequestContract request)
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

        public PagedResultList<RoleContract> GetRolesByProject(int projectId, int start, int count, string query)
        {
            try
            {
                var url = UrlQueryBuilder.Create($"project/{projectId}/role")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByName", query)
                    .ToQuery();

                var result = m_client.GetPagedList<RoleContract>(url);
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
    }
}
