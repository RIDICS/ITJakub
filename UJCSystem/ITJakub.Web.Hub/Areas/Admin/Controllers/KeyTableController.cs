using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

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

        public IActionResult KeyTableType(KeyTableEditorType editorType)
        {
            switch (editorType)
            {
                case KeyTableEditorType.Category:
                    return PartialView("KeyTableEditors/_Category");
                case KeyTableEditorType.Genre:
                    return PartialView("KeyTableEditors/_Genre");
                case KeyTableEditorType.Kind:
                    return PartialView("KeyTableEditors/_Kind");
                case KeyTableEditorType.ResponsiblePerson:
                    return PartialView("KeyTableEditors/_ResponsiblePerson");
                case KeyTableEditorType.ResponsiblePersonEditor:
                    return PartialView("KeyTableEditors/_ResponsiblePersonEditor");
                case KeyTableEditorType.LiteraryOriginal:
                    return PartialView("KeyTableEditors/_LiteraryOriginal");
                case KeyTableEditorType.OriginalAuthor:
                    return PartialView("KeyTableEditors/_OriginalAuthor");
                case KeyTableEditorType.Keyword:
                    return PartialView("KeyTableEditors/_Keyword");
                default:
                    return PartialView("KeyTableEditors/_Category");
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
        public IActionResult CreateLiteraryGenre(LiteraryGenreContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateLiteraryGenre(request);
                return Json(newId);
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
        public IActionResult CreateResponsiblePerson(ResponsiblePersonContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateResponsiblePerson(request);
                return Json(newId);
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

        public IActionResult GetResponsibleTypeList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetResponsibleTypeList();
                return Json(result);
            }
        }

        [HttpGet]
        public IActionResult GetLitararyGenreList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetLitararyGenreList();
                return Json(result);
            }
        }

        [HttpGet]
        public IActionResult GetLiteraryKindList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetLiteraryKindList();
                return Json(result);
            }
        }

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

        [HttpGet]
        public IActionResult GetLitararyOriginalList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetLitararyOriginalList();
                return Json(result);
            }
        }
    }
}