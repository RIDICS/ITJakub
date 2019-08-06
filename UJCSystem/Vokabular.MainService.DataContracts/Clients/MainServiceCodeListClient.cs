using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceCodeListClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceCodeListClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        #region Category

        public int CreateCategory(CategoryContract category)
        {
            try
            {
                var resultId = m_client.Post<int>("category", category);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public object UpdateCategory(int categoryId, CategoryContract category)
        {
            try
            {
                var resultId = m_client.Put<object>($"category/{categoryId}", category);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteCategory(int categoryId)
        {
            try
            {
                m_client.Delete($"category/{categoryId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CategoryContract> GetCategoryList()
        {
            try
            {
                var result = m_client.Get<List<CategoryContract>>("category");
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

        #region Genre

        public int CreateLiteraryGenre(LiteraryGenreContract literaryGenre)
        {
            try
            {
                var newId = m_client.Post<int>("literarygenre", literaryGenre);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateLiteraryGenre(int literaryGenreId, LiteraryGenreContract data)
        {
            try
            {
                m_client.Put<HttpStatusCode>($"literarygenre/{literaryGenreId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteLiteraryGenre(int literaryGenreId)
        {
            try
            {
                m_client.Delete($"literarygenre/{literaryGenreId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            try
            {
                var result = m_client.Get<List<LiteraryGenreContract>>("literarygenre");
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

        #region Keyword

        public List<KeywordContract> GetKeywordAutocomplete(string query, int? count)
        {
            try
            {
                var result = m_client.Get<List<KeywordContract>>($"keyword/autocomplete?query={query}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<KeywordContract> GetKeywordList(int? start, int? count)
        {
            try
            {
                var result = m_client.GetPagedList<KeywordContract>($"keyword?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateKeyword(KeywordContract keyword)
        {
            try
            {
                var newId = m_client.Post<int>("keyword", keyword);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateKeyword(int keywordId, KeywordContract data)
        {
            try
            {
                m_client.Put<HttpStatusCode>($"keyword/{keywordId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteKeyword(int keywordId)
        {
            try
            {
                m_client.Delete($"keyword/{keywordId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Literary Original

        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            try
            {
                var result = m_client.Get<List<LiteraryOriginalContract>>("literaryoriginal");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateLiteraryOriginal(LiteraryOriginalContract literaryOriginal)
        {
            try
            {
                var newId = m_client.Post<int>("literaryoriginal", literaryOriginal);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateLiteraryOriginal(int literaryOriginalId, LiteraryOriginalContract data)
        {
            try
            {
                m_client.Put<HttpStatusCode>($"literaryoriginal/{literaryOriginalId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteLiteraryOriginal(int literaryOriginalId)
        {
            try
            {
                m_client.Delete($"literaryoriginal/{literaryOriginalId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Literary Kind

        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            try
            {
                var result = m_client.Get<List<LiteraryKindContract>>("literarykind");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateLiteraryKind(LiteraryKindContract literaryKind)
        {
            try
            {
                var newId = m_client.Post<int>("literarykind", literaryKind);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateLiteraryKind(int literaryKindId, LiteraryKindContract data)
        {
            try
            {
                m_client.Put<HttpStatusCode>($"literarykind/{literaryKindId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteLiteraryKind(int literaryKindId)
        {
            try
            {
                m_client.Delete($"literarykind/{literaryKindId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Original author

        public PagedResultList<ProjectDetailContract> GetProjectsByAuthor(int authorId, int? start, int? count)
        {
            try
            {
                var result = m_client.GetPagedList<ProjectDetailContract>($"author/{authorId}/project?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<OriginalAuthorContract> GetOriginalAuthorList(int start, int count)
        {
            try
            {
                var result = m_client.GetPagedList<OriginalAuthorContract>($"author?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateOriginalAuthor(OriginalAuthorContract author)
        {
            try
            {
                var newId = m_client.Post<int>("author", author);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateOriginalAuthor(int authorId, OriginalAuthorContract data)
        {
            try
            {
                m_client.Put<HttpStatusCode>($"author/{authorId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteOriginalAuthor(int authorId)
        {
            try
            {
                m_client.Delete($"author/{authorId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Responsible person

        public PagedResultList<ProjectDetailContract> GetProjectsByResponsiblePerson(int responsiblePersonId, int? start, int? count)
        {
            try
            {
                var result = m_client.GetPagedList<ProjectDetailContract>(
                    $"responsibleperson/{responsiblePersonId}/project?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateResponsiblePerson(ResponsiblePersonContract responsiblePerson)
        {
            try
            {
                var newId = m_client.Post<int>("responsibleperson", responsiblePerson);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<ResponsiblePersonContract> GetResponsiblePersonList(int start, int count)
        {
            try
            {
                var result = m_client.GetPagedList<ResponsiblePersonContract>($"responsibleperson?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateResponsiblePerson(int responsiblePersonId, ResponsiblePersonContract data)
        {
            try
            {
                m_client.Put<HttpStatusCode>($"responsibleperson/{responsiblePersonId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteResponsiblePerson(int responsiblePersonId)
        {
            try
            {
                m_client.Delete($"responsibleperson/{responsiblePersonId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Responsible person type

        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            try
            {
                var result = m_client.Get<List<ResponsibleTypeContract>>("responsibleperson/type");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateResponsibleType(ResponsibleTypeContract responsibleType)
        {
            try
            {
                var resultId = m_client.Post<int>("responsibleperson/type", responsibleType);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteResponsibleType(int responsibleTypeId)
        {
            try
            {
                m_client.Delete($"responsibleperson/type/{responsibleTypeId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateResponsibleType(int responsibleTypeId, ResponsibleTypeContract data)
        {
            try
            {
                m_client.Put<HttpStatusCode>($"responsibleperson/type/{responsibleTypeId}", data);
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
