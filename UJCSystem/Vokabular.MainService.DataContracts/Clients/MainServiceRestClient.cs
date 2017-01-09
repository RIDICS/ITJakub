using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.MainService.DataContracts.Data;
using Vokabular.MainService.DataContracts.ServiceContracts;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceRestClient : RestClientBase, IProjectMainService, IResourceMainService, ISnapshotMainService
    {
        public MainServiceRestClient(Uri baseAddress) : base(baseAddress)
        {
        }

        public List<ProjectContract> GetProjectList(int? start, int? count)
        {
            if (start == null || count == null)
                throw new ArgumentException("start or count argument is null");

            return GetProjectListFull(start.Value, count.Value).List;
        }

        public ProjectListData GetProjectListFull(int start, int count)
        {
            var result = GetFull<List<ProjectContract>>($"project?start={start}&count={count}");
            return new ProjectListData
            {
                TotalCount = result.GetTotalCountHeader(),
                List = result.Result
            };
        }

        public ProjectContract GetProject(long projectId)
        {
            var project = Get<ProjectContract>($"project/{projectId}");
            return project;
        }

        public long CreateProject(ProjectContract project)
        {
            var projectId = Post<long>("project", project);
            return projectId;
        }

        public void DeleteProject(long projectId)
        {
            Delete($"project/{projectId}");
        }

        public ProjectMetadataContract GetProjectMetadata(long projectId)
        {
            var metadata = Get<ProjectMetadataContract>($"project/{projectId}/metadata");
            return metadata;
        }

        public void UploadResource(string sessionId, Stream data)
        {
            var uriPath = $"session/{sessionId}/resource";
            var content = new StreamContent(data);
            var response = HttpClient.PostAsync(uriPath, content).Result;

            response.EnsureSuccessStatusCode();
        }

        public void DeleteResource(long resourceId)
        {
            Delete($"resource/{resourceId}");
        }

        public long DuplicateResource(long resourceId)
        {
            var newResourceId = Post<long>($"resource/{resourceId}/duplicate", null);
            return newResourceId;
        }

        public List<ResourceContract> GetResourceList(long projectId, ResourceTypeContract? resourceType = null)
        {
            var url = $"project/{projectId}/resource";
            if (resourceType != null)
            {
                url = $"{url}?resourceType={resourceType}";
            }

            var result = Get<List<ResourceContract>>(url);
            return result;
        }

        public List<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            var result = Get<List<ResourceVersionContract>>($"resource/{resourceId}/version");
            return result;
        }

        public ResourceMetadataContract GetResourceMetadata(long resourceId)
        {
            var result = Get<ResourceMetadataContract>($"resource/{resourceId}/metadata");
            return result;
        }

        public long ProcessUploadedResources(long projectId, NewResourceContract resourceInfo)
        {
            var resourceId = Post<long>($"project/{projectId}/resource", resourceInfo);
            return resourceId;
        }

        public long ProcessUploadedResourceVersion(long resourceId, NewResourceContract resourceInfo)
        {
            var resourceVersionId = Post<long>($"resource/{resourceId}/version", resourceInfo);
            return resourceVersionId;
        }

        public void RenameResource(long resourceId, ResourceContract resource)
        {
            Put($"resource/{resourceId}", resource);
        }

        public void UploadResource(string sessionId)
        {
            throw new NotSupportedException("Method without Stream parameter is not supported");
        }

        public long CreateSnapshot(long projectId)
        {
            var snapshotId = Post<long>($"project/{projectId}/snapshot", null);
            return snapshotId;
        }

        public List<SnapshotContract> GetSnapshotList(long projectId)
        {
            var result = Get<List<SnapshotContract>>($"project/{projectId}/snapshot");
            return result;
        }

        public int CreatePublisher(PublisherContract publisher)
        {
            var newId = Post<int>("publisher", publisher);
            return newId;
        }

        public int CreateLiteraryKind(LiteraryKindContract literaryKind)
        {
            var newId = Post<int>("literarykind", literaryKind);
            return newId;
        }

        public int CreateLiteraryGenre(LiteraryGenreContract literaryGenre)
        {
            var newId = Post<int>("literarygenre", literaryGenre);
            return newId;
        }

        public int CreateOriginalAuthor(OriginalAuthorContract author)
        {
            var newId = Post<int>("author", author);
            return newId;
        }

        public int CreateResponsiblePerson(ResponsiblePersonContract responsiblePerson)
        {
            var newId = Post<int>("responsibleperson", responsiblePerson);
            return newId;
        }

        public List<PublisherContract> GetPublisherList()
        {
            var result = Get<List<PublisherContract>>("publisher");
            return result;
        }

        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = Get<List<LiteraryKindContract>>("literarykind");
            return result;
        }

        public List<LiteraryGenreContract> GetLitararyGenreList()
        {
            var result = Get<List<LiteraryGenreContract>>("literarygenre");
            return result;
        }
    }
}
