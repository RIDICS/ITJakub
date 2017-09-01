using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Clients.Extensions;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.MainService.DataContracts.Data;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceRestClient : FullRestClientBase
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();

        public MainServiceRestClient(Uri baseAddress) : base(baseAddress)
        {
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }

        public ProjectListData GetProjectList(int start, int count)
        {
            try
            {
                var result = GetFull<List<ProjectContract>>($"project?start={start}&count={count}");
                return new ProjectListData
                {
                    TotalCount = result.GetTotalCountHeader(),
                    List = result.Result
                };
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public ProjectContract GetProject(long projectId)
        {
            try
            {
                var project = Get<ProjectContract>($"project/{projectId}");
                return project;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateProject(ProjectContract project)
        {
            try
            {
                var projectId = Post<long>("project", project);
                return projectId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteProject(long projectId)
        {
            try
            {
                Delete($"project/{projectId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public ProjectMetadataResultContract GetProjectMetadata(long projectId, bool includeAuthor, bool includeResponsiblePerson, bool includeKind, bool includeGenre)
        {
            try
            {
                var metadata =
                    Get<ProjectMetadataResultContract>(
                        $"project/{projectId}/metadata?includeAuthor={includeAuthor}&includeResponsiblePerson={includeResponsiblePerson}&includeKind={includeKind}&includeGenre={includeGenre}");
                return metadata;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateNewProjectMetadataVersion(long projectId, ProjectMetadataContract metadata)
        {
            try
            {
                var newResourceVersion = Post<long>($"project/{projectId}/metadata", metadata);
                return newResourceVersion;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        public void SetProjectLiteraryKinds(long projectId, IntegerIdListContract request)
        {
            try
            {
                Put<object>($"project/{projectId}/literarykind", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        public void SetProjectLiteraryGenres(long projectId, IntegerIdListContract request)
        {
            try
            {
                Put<object>($"project/{projectId}/literarygenre", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        public void SetProjectAuthors(long projectId, IntegerIdListContract request)
        {
            try
            {
                Put<object>($"project/{projectId}/author", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetProjectResponsiblePersons(long projectId, IntegerIdListContract request)
        {
            try
            {
                Put<object>($"project/{projectId}/responsibleperson", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UploadResource(string sessionId, Stream data, string fileName)
        {
            try
            {
                var uriPath = $"session/{sessionId}/resource".AddQueryString("fileName", fileName);
                PostStream<object>(uriPath, data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void ProcessSessionAsImport(string sessionId, NewBookImportContract request)
        {
            try
            {
                HttpClient.Timeout = new TimeSpan(0, 10, 0); // Import is long running operation
                Post<object>($"session/{sessionId}", request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteResource(long resourceId)
        {
            try
            {
                Delete($"resource/{resourceId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long DuplicateResource(long resourceId)
        {
            try
            {
                var newResourceId = Post<long>($"resource/{resourceId}/duplicate", null);
                return newResourceId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

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

                var result = Get<List<ResourceContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            try
            {
                var result = Get<List<ResourceVersionContract>>($"resource/{resourceId}/version");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public ResourceMetadataContract GetResourceMetadata(long resourceId)
        {
            try
            {
                var result = Get<ResourceMetadataContract>($"resource/{resourceId}/metadata");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long ProcessUploadedResources(long projectId, NewResourceContract resourceInfo)
        {
            try
            {
                var resourceId = Post<long>($"project/{projectId}/resource", resourceInfo);
                return resourceId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long ProcessUploadedResourceVersion(long resourceId, NewResourceContract resourceInfo)
        {
            try
            {
                var resourceVersionId = Post<long>($"resource/{resourceId}/version", resourceInfo);
                return resourceVersionId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void RenameResource(long resourceId, ResourceContract resource)
        {
            try
            {
                Put<object>($"resource/{resourceId}", resource);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        public long CreateSnapshot(long projectId)
        {
            try
            {
                var snapshotId = Post<long>($"project/{projectId}/snapshot", null);
                return snapshotId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<SnapshotContract> GetSnapshotList(long projectId)
        {
            try
            {
                var result = Get<List<SnapshotContract>>($"project/{projectId}/snapshot");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateLiteraryKind(LiteraryKindContract literaryKind)
        {
            try
            {
                var newId = Post<int>("literarykind", literaryKind);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateLiteraryGenre(LiteraryGenreContract literaryGenre)
        {
            try
            {
                var newId = Post<int>("literarygenre", literaryGenre);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateOriginalAuthor(OriginalAuthorContract author)
        {
            try
            {
                var newId = Post<int>("author", author);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateResponsiblePerson(NewResponsiblePersonContract responsiblePerson)
        {
            try
            {
                var newId = Post<int>("responsibleperson", responsiblePerson);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            try
            {
                var result = Get<List<LiteraryKindContract>>("literarykind");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<LiteraryGenreContract> GetLitararyGenreList()
        {
            try
            {
                var result = Get<List<LiteraryGenreContract>>("literarygenre");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateCategory(CategoryContract category)
        {
            try
            {
                var resultId = Post<int>("category", category);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CategoryContract> GetCategoryList()
        {
            try
            {
                var result = Get<List<CategoryContract>>("category");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        public int CreateResponsibleType(ResponsibleTypeContract responsibleType)
        {
            try
            {
                var resultId = Post<int>("responsibleperson/type", responsibleType);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            try
            {
                var result = Get<List<ResponsibleTypeContract>>("responsibleperson/type");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<OriginalAuthorContract> GetOriginalAuthorAutocomplete(string query)
        {
            try
            {
                var result = Get<List<OriginalAuthorContract>>("author/autocomplete".AddQueryString("query", query));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ResponsiblePersonContract> GetResponsiblePersonAutocomplete(string query)
        {
            try
            {
                var result = Get<List<ResponsiblePersonContract>>("responsibleperson/autocomplete".AddQueryString("query", query));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
