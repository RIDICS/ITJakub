using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KeyTableController : BaseController
    {
        public KeyTableController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public IActionResult KeyTable()
        {
            return View();
        }

        #region Category
        [HttpGet]
        public IActionResult GetCategoryList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetCategoryList();
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryContract category)
        {
            using (var client = GetRestClient())
            {
                var result = client.CreateCategory(category);
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult RenameCategory(int categoryId, CategoryContract category)
        {
            using (var client = GetRestClient())
            {
                var result = client.UpdateCategory(categoryId, category);
                return Json(result);
            }
        }
        [HttpPost]
        public void DeleteCategory(int categoryId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteCategory(categoryId);
            }
        }
        #endregion
        #region Genre
        [HttpPost]
        public IActionResult CreateLiteraryGenre(LiteraryGenreContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateLiteraryGenre(request);
                return Json(newId);
            }
        }


        [HttpGet]
        public IActionResult GetLiteraryGenreList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetLiteraryGenreList();
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult RenameLiteraryGenre(int literaryGenreId, LiteraryGenreContract data)
        {
            using (var client = GetRestClient())
            {
                var response = client.UpdateLiteraryGenre(literaryGenreId, data);
                return Json(response);
            }
        }

        [HttpPost]
        public void DeleteLiteraryGenre(int literaryGenreId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteLiteraryGenre(literaryGenreId);
            }
        }
        #endregion
        #region Kind
        [HttpGet]
        public IActionResult GetLiteraryKindList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetLiteraryKindList();
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult CreateLiteraryKind(LiteraryKindContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateLiteraryKind(request);
                return Json(newId);
            }
        }
        [HttpPost]
        public void DeleteLiteraryKind(int literaryKindId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteLiteraryKind(literaryKindId);
            }
        }
        [HttpPost]
        public IActionResult RenameLiteraryKind(int literaryKindId, LiteraryKindContract request)
        {
            using (var client = GetRestClient())
            {
                var response = client.UpdateLiteraryKind(literaryKindId, request);
                return Json(response);
            }
        }
        #endregion
        #region Responsible person type
        [HttpGet]
        public IActionResult GetResponsibleTypeList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetResponsibleTypeList();
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult CreateResponsibleType(ResponsibleTypeContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateResponsibleType(request);
                return Json(newId);
            }
        }
        [HttpPost]
        public void DeleteResponsibleType(int responsibleTypeId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteResponsibleType(responsibleTypeId);
            }
        }
        [HttpPost]
        public IActionResult RenameResponsibleType(int responsibleTypeId, ResponsibleTypeContract data)
        {
            using (var client = GetRestClient())
            {
                var response = client.UpdateResponsibleType(responsibleTypeId, data);
                return Json(response);
            }
        }
        #endregion
        #region Responsible person
        [HttpPost]
        public IActionResult CreateResponsiblePerson(ResponsiblePersonContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateResponsiblePerson(request);
                return Json(newId);
            }
        }
        [HttpGet]
        public IActionResult GetResponsiblePersonList(int start, int count)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetResponsiblePersonList(start, count);
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult RenameResponsiblePerson(int responsiblePersonId, ResponsiblePersonContract request)
        {
            using (var client = GetRestClient())
            {
                var response = client.UpdateResponsiblePerson(responsiblePersonId, request);
                return Json(response);
            }
        }
        [HttpPost]
        public void DeleteResponsiblePerson(int responsiblePersonId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteResponsiblePerson(responsiblePersonId);
            }
        }
        #endregion
        #region Literary original
        [HttpGet]
        public IActionResult GetLiteraryOriginalList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetLiteraryOriginalList();
                return Json(result);
            }
        }
        [HttpPost]
        public void DeleteLiteraryOriginal(int literaryOriginalId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteLiteraryOriginal(literaryOriginalId);
            }
        }
        [HttpPost]
        public IActionResult CreateLiteraryOriginal(LiteraryOriginalContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateLiteraryOriginal(request);
                return Json(newId);
            }
        }
        [HttpPost]
        public IActionResult RenameLiteraryOriginal(int literaryOriginalId, LiteraryOriginalContract request)
        {
            using (var client = GetRestClient())
            {
                var response = client.UpdateLiteraryOriginal(literaryOriginalId, request);
                return Json(response);
            }
        }
        #endregion
        #region Original author
        [HttpGet]
        public IActionResult GetOriginalAuthorList(int start, int count)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetOriginalAuthorList(start, count);
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult CreateAuthor(OriginalAuthorContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateOriginalAuthor(request);
                return Json(newId);
            }
        }
        [HttpPost]
        public IActionResult RenameOriginalAuthor(int authorId, OriginalAuthorContract request)
        {
            using (var client = GetRestClient())
            {
                var response = client.UpdateOriginalAuthor(authorId, request);
                return Json(response);
            }
        }
        [HttpPost]
        public void DeleteOriginalAuthor(int authorId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteOriginalAuthor(authorId);
            }
        }
        #endregion
        #region Keyword
        [HttpGet]
        public IActionResult GetKeywordList(int start, int count)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetKeywordList(start, count);
                return Json(result);
            }
        }
        [HttpPost]
        public void DeleteKeyword(int keywordId)
        {
            using (var client = GetRestClient())
            {
                client.DeleteKeyword(keywordId);
            }
        }
        [HttpPost]
        public IActionResult CreateKeyword(KeywordContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateKeyword(request);
                return Json(newId);
            }
        }
        [HttpPost]
        public IActionResult RenameKeyword(int keywordId, KeywordContract request)
        {
            using (var client = GetRestClient())
            {
                var response = client.UpdateKeyword(keywordId, request);
                return Json(response);
            }
        }
        #endregion
    }
}