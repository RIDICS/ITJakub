using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.MainService.DataContracts.Data;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceRestClient : RestClientBase
    {
        public MainServiceRestClient(Uri baseAddress) : base(baseAddress)
        {
        }
        
        public ProjectListData GetProjectList(int start, int count)
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

        public ProjectMetadataResultContract GetProjectMetadata(long projectId, bool includeAuthor, bool includeResponsiblePerson, bool includeKind, bool includeGenre)
        {
            var metadata =
                Get<ProjectMetadataResultContract>(
                    $"project/{projectId}/metadata?includeAuthor={includeAuthor}&includeResponsiblePerson={includeResponsiblePerson}&includeKind={includeKind}&includeGenre={includeGenre}");
            return metadata;
        }

        public long CreateNewProjectMetadataVersion(long projectId, ProjectMetadataContract metadata)
        {
            var newResourceVersion = Post<long>($"project/{projectId}/metadata", metadata);
            return newResourceVersion;
        }
        
        public void SetProjectLiteraryKinds(long projectId, IntegerIdListContract request)
        {
            Put($"project/{projectId}/literarykind", request);
        }
        
        public void SetProjectLiteraryGenres(long projectId, IntegerIdListContract request)
        {
            Put($"project/{projectId}/literarygenre", request);
        }
        
        public void SetProjectAuthors(long projectId, IntegerIdListContract request)
        {
            Put($"project/{projectId}/author", request);
        }

        public void SetProjectResponsiblePersons(long projectId, IntegerIdListContract request)
        {
            Put($"project/{projectId}/responsibleperson", request);
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

        public int CreateResponsiblePerson(NewResponsiblePersonContract responsiblePerson)
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

        public int CreateCategory(CategoryContract category)
        {
            var resultId = Post<int>("category", category);
            return resultId;
        }

        public List<CategoryContract> GetCategoryList()
        {
            var result = Get<List<CategoryContract>>("category");
            return result;
        }
        
        public int CreateResponsibleType(ResponsibleTypeContract responsibleType)
        {
            var resultId = Post<int>("responsibleperson/type", responsibleType);
            return resultId;
        }

        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            var result = Get<List<ResponsibleTypeContract>>("responsibleperson/type");
            return result;
        }

        public List<OriginalAuthorContract> GetOriginalAuthorAutocomplete(string query)
        {
            var result = Get<List<OriginalAuthorContract>>($"author/autocomplete?query={query}");
            return result;
        }

        public List<ResponsiblePersonContract> GetResponsiblePersonAutocomplete(string query)
        {
            var result = Get<List<ResponsiblePersonContract>>($"responsibleperson/autocomplete?query={query}");
            return result;
        }
    }
}
