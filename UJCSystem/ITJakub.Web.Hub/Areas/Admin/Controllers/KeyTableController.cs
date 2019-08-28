using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.Const;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(VokabularPermissionNames.ManageCodeList)]
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
            var client = GetCodeListClient();
            var result = client.GetCategoryList();
            return Json(result);
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryContract category)
        {
            var client = GetCodeListClient();
            var result = client.CreateCategory(category);
            return Json(result);
        }

        [HttpPost]
        public IActionResult RenameCategory(int categoryId, CategoryContract category)
        {
            var client = GetCodeListClient();
            var result = client.UpdateCategory(categoryId, category);
            return Json(result);
        }

        [HttpPost]
        public void DeleteCategory(int categoryId)
        {
            var client = GetCodeListClient();
            client.DeleteCategory(categoryId);
        }

        #endregion

        #region Genre

        [HttpPost]
        public IActionResult CreateLiteraryGenre(LiteraryGenreContract request)
        {
            var client = GetCodeListClient();
            var newId = client.CreateLiteraryGenre(request);
            return Json(newId);
        }


        [HttpGet]
        public IActionResult GetLiteraryGenreList()
        {
            var client = GetCodeListClient();
            var result = client.GetLiteraryGenreList();
            return Json(result);
        }

        [HttpPost]
        public void RenameLiteraryGenre(int literaryGenreId, LiteraryGenreContract data)
        {
            var client = GetCodeListClient();
            client.UpdateLiteraryGenre(literaryGenreId, data);
        }

        [HttpPost]
        public void DeleteLiteraryGenre(int literaryGenreId)
        {
            var client = GetCodeListClient();
            client.DeleteLiteraryGenre(literaryGenreId);
        }

        #endregion

        #region Kind

        [HttpGet]
        public IActionResult GetLiteraryKindList()
        {
            var client = GetCodeListClient();
            var result = client.GetLiteraryKindList();
            return Json(result);
        }

        [HttpPost]
        public IActionResult CreateLiteraryKind(LiteraryKindContract request)
        {
            var client = GetCodeListClient();
            var newId = client.CreateLiteraryKind(request);
            return Json(newId);
        }

        [HttpPost]
        public void DeleteLiteraryKind(int literaryKindId)
        {
            var client = GetCodeListClient();
            client.DeleteLiteraryKind(literaryKindId);
        }

        [HttpPost]
        public void RenameLiteraryKind(int literaryKindId, LiteraryKindContract request)
        {
            var client = GetCodeListClient();
            client.UpdateLiteraryKind(literaryKindId, request);
        }

        #endregion

        #region Responsible person type

        [HttpGet]
        public IActionResult GetResponsibleTypeList()
        {
            var client = GetCodeListClient();
            var result = client.GetResponsibleTypeList();
            return Json(result);
        }

        [HttpPost]
        public IActionResult CreateResponsibleType(ResponsibleTypeContract request)
        {
            var client = GetCodeListClient();
            var newId = client.CreateResponsibleType(request);
            return Json(newId);
        }

        [HttpPost]
        public void DeleteResponsibleType(int responsibleTypeId)
        {
            var client = GetCodeListClient();
            client.DeleteResponsibleType(responsibleTypeId);
        }

        [HttpPost]
        public void RenameResponsibleType(int responsibleTypeId, ResponsibleTypeContract data)
        {
            var client = GetCodeListClient();
            client.UpdateResponsibleType(responsibleTypeId, data);
        }

        #endregion

        #region Responsible person

        [HttpPost]
        public IActionResult CreateResponsiblePerson(ResponsiblePersonContract request)
        {
            var client = GetCodeListClient();
            var newId = client.CreateResponsiblePerson(request);
            return Json(newId);
        }

        [HttpGet]
        public IActionResult GetResponsiblePersonList(int start, int count)
        {
            var client = GetCodeListClient();
            var result = client.GetResponsiblePersonList(start, count);
            return Json(result);
        }

        [HttpPost]
        public void RenameResponsiblePerson(int responsiblePersonId, ResponsiblePersonContract request)
        {
            var client = GetCodeListClient();
            client.UpdateResponsiblePerson(responsiblePersonId, request);
        }

        [HttpPost]
        public void DeleteResponsiblePerson(int responsiblePersonId)
        {
            var client = GetCodeListClient();
            client.DeleteResponsiblePerson(responsiblePersonId);
        }

        #endregion

        #region Literary original

        [HttpGet]
        public IActionResult GetLiteraryOriginalList()
        {
            var client = GetCodeListClient();
            var result = client.GetLiteraryOriginalList();
            return Json(result);
        }

        [HttpPost]
        public void DeleteLiteraryOriginal(int literaryOriginalId)
        {
            var client = GetCodeListClient();
            client.DeleteLiteraryOriginal(literaryOriginalId);
        }

        [HttpPost]
        public IActionResult CreateLiteraryOriginal(LiteraryOriginalContract request)
        {
            var client = GetCodeListClient();
            var newId = client.CreateLiteraryOriginal(request);
            return Json(newId);
        }

        [HttpPost]
        public void RenameLiteraryOriginal(int literaryOriginalId, LiteraryOriginalContract request)
        {
            var client = GetCodeListClient();
            client.UpdateLiteraryOriginal(literaryOriginalId, request);
        }

        #endregion

        #region Original author

        [HttpGet]
        public IActionResult GetOriginalAuthorList(int start, int count)
        {
            var client = GetCodeListClient();
            var result = client.GetOriginalAuthorList(start, count);
            return Json(result);
        }

        [HttpPost]
        public IActionResult CreateAuthor(OriginalAuthorContract request)
        {
            var client = GetCodeListClient();
            var newId = client.CreateOriginalAuthor(request);
            return Json(newId);
        }

        [HttpPost]
        public void RenameOriginalAuthor(int authorId, OriginalAuthorContract request)
        {
            var client = GetCodeListClient();
            client.UpdateOriginalAuthor(authorId, request);
        }

        [HttpPost]
        public void DeleteOriginalAuthor(int authorId)
        {
            var client = GetCodeListClient();
            client.DeleteOriginalAuthor(authorId);
        }

        #endregion

        #region Keyword

        [HttpGet]
        public IActionResult GetKeywordList(int start, int count)
        {
            var client = GetCodeListClient();
            var result = client.GetKeywordList(start, count);
            return Json(result);
        }

        [HttpPost]
        public void DeleteKeyword(int keywordId)
        {
            var client = GetCodeListClient();
            client.DeleteKeyword(keywordId);
        }

        [HttpPost]
        public IActionResult CreateKeyword(KeywordContract request)
        {
            var client = GetCodeListClient();
            var newId = client.CreateKeyword(request);
            return Json(newId);
        }

        [HttpPost]
        public void RenameKeyword(int keywordId, KeywordContract request)
        {
            var client = GetCodeListClient();
            client.UpdateKeyword(keywordId, request);
        }

        #endregion
    }
}