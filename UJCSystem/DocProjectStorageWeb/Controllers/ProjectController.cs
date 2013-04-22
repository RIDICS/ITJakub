using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocProjectStorageWeb.Models;
using DocProjectStorageWeb.WebModels;
using Newtonsoft.Json;
using System.IO;
using DocProjectStorageWeb.AccountsServiceReference;

namespace DocProjectStorageWeb.Controllers
{
    public class ProjectController : Controller
    {

        private static DocProjectModelContainer container = new DocProjectModelContainer();
        private IAccountsService accSvc = new AccountsServiceClient();

       

        private DocProjectEntity FindProject(int id)
        {
            var proj = from p in container.DocProjectEntities
                       where p.Id == id
                       select p;

            if (proj == null)
            {
                return null;
            }

            DocProjectEntity projObj = proj.FirstOrDefault<DocProjectEntity>();
            return projObj;
        }

        private ProjectModel FindAndTransformProject(int id)
        {
            DocProjectEntity projObj = FindProject(id);

            ProjectModel project = new ProjectModel();
            project.Id = projObj.Id;

            project.Title = projObj.Title;
            project.Author = projObj.Author;
            project.DocType = projObj.Type.Name;

            project.Editors = new List<string>();
            foreach (EditorEntity editor in projObj.Editors)
            {
                DocProjectStorageWeb.AccountsServiceReference.User e = accSvc.GetUserByEmail(editor.User.Email);
                string editorName = DocProjectUtils.ConstructName(e);
                project.Editors.Add(editorName);
            }

            // responsible redactors
            project.ResponsibleRedactors = new List<string>();
            var allUsers = from u in container.UserEntities
                           select u;
            List<UserEntity> allUsersObj = allUsers.ToList();
            string expProjectType = projObj.Type.ToString();
            string expRoleName = Role.REDACTOR.ToString();
            foreach (UserEntity u in allUsersObj)
            {
                foreach (UserRoleEntity r in u.Roles)
                {
                    if (r.RoleName.Equals(DocProjectUtils.ConstructRoleName(expRoleName,expProjectType)))
                    {
                        string name = DocProjectUtils.ConstructName(accSvc.GetUserByEmail(u.Email));
                        project.ResponsibleRedactors.Add(name);
                    }
                }
            }

            // responsible technical redactors
            project.ResponsibleTechRedactors = new List<string>();
            var allUsers2 = from u in container.UserEntities
                            select u;
            List<UserEntity> allUsersObj2 = allUsers.ToList();
            string expProjectType2 = projObj.Type.ToString();
            string expRoleName2 = Role.TECHNICAL_REDACTOR.ToString();
            foreach (UserEntity u in allUsersObj)
            {
                foreach (UserRoleEntity r in u.Roles)
                {
                    if (r.RoleName.Equals(DocProjectUtils.ConstructRoleName(expRoleName2, expProjectType2)))
                    {
                        string name = DocProjectUtils.ConstructName(accSvc.GetUserByEmail(u.Email));
                        project.ResponsibleTechRedactors.Add(name);
                    }
                }
            }

            project.ProjectState = (ProjectState)Enum.Parse(typeof(ProjectState), projObj.ProjectState.Type, true);
            project.LockingPerson = "Boris Lehečka"; // TODO

            // Revision
            project.Revisions = new List<Revision>();
            foreach (RevisionEntity rev in projObj.Revisions)
            {
                Revision r = new Revision();
                r.RevNumber = rev.Number;
                r.ReleaseDate = rev.Released;
                project.Revisions.Add(r);
            }

            return project;
        }

        public ActionResult Index(int id)
        {
            ProjectModel model = FindAndTransformProject(id);
            return View(model);
        }

        /// <summary>
        /// This is controller method for new project form
        /// </summary>
        /// <returns></returns>
        public ActionResult New()
        {
            DocProjectStorageWeb.WebModels.NewProjectModel model = new DocProjectStorageWeb.WebModels.NewProjectModel();
            List<SelectListItem> docTypeItems = new List<SelectListItem>();

            List<DocumentType> docTypes = DocProjectUtils.GetAllDocumentTypes();

            foreach (DocumentType docType in docTypes)
            {
                SelectListItem item = new SelectListItem();
                item.Text = docType.Name;
                item.Value = docType.Name;
                docTypeItems.Add(item);
            }

            model.DocTypes = docTypeItems.ToArray();

            return View(model);
        }

        /// <summary>
        /// Method that establish new project in a database
        /// </summary>
        /// <returns></returns>
        public ActionResult EstablishProject(NewProjectModel model,  HttpPostedFileBase file)
        {
            DocProjectEntity project = new DocProjectEntity();
            project.Title = model.Title;
            project.Author = model.Author;
            project.Type = GetDocType(model.DocTypeValue);

            ProjectStateEntity projectState = new ProjectStateEntity();
            projectState.Type = ProjectState.EDITOR.ToString();
            project.ProjectState = projectState;

            string email = User.Identity.Name;
            UserEntity user = GetUserByEmail(email);

            EditorEntity editor = new EditorEntity();
            editor.CanWrite = true;
            editor.User = user;

            project.Editors.Add(editor);

            RevisionEntity defaultRevision = new RevisionEntity();
            defaultRevision.Number = 1; // TODO constant?
            defaultRevision.Released = DateTime.Now;
            using (MemoryStream ms = new MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                defaultRevision.DocxData = ms.GetBuffer();
            }
            project.Revisions.Add(defaultRevision);

            container.DocProjectEntities.Add(project);
            container.SaveChanges(); // return value should be checked ^^

            return RedirectToAction("Index", "Home");
        }

        private UserEntity GetUserByEmail(string email)
        {
            var users = from p in container.UserEntities
                        where p.Email == email
                        select p;

            UserEntity user = users.FirstOrDefault<UserEntity>();
            return user;
        }

        /// <summary>
        /// Returns object of the project type. Returs existing if some exists, otherwise creates the new one
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private DocumentType GetDocType(string name)
        {
            var types = from t in container.DocumentTypes
                        where t.Name == name
                        select t;

            
            List<DocumentType> list = types.ToList<DocumentType>();

            DocumentType type;
            if (list.Count == 0)
            {
                type= new DocumentType();
                type.Name = name;
                return type;
            }

            type = types.FirstOrDefault<DocumentType>();
            return type;
        }
        
        public ActionResult FetchEditors(int id, string email)
        {
                var project = from p in container.DocProjectEntities
                          where p.Id == id
                          select p;
            DocProjectEntity proj = project.FirstOrDefault<DocProjectEntity>();


            List<string> editors= new List<string>();
            foreach (EditorEntity editor in proj.Editors)
            {
                DocProjectStorageWeb.AccountsServiceReference.User e = accSvc.GetUserByEmail(editor.User.Email);
                string editorName = DocProjectUtils.ConstructName(e);
                editors.Add(editorName);
            }

            string serialized = JsonConvert.SerializeObject(editors);
            JsonResult result = Json(serialized, "application/json", JsonRequestBehavior.AllowGet);
            return result;
        }

        public ActionResult FetchMessages(int id)
        {
            var project = from p in container.DocProjectEntities
                          where p.Id == id
                          select p;
            DocProjectEntity proj = project.FirstOrDefault<DocProjectEntity>();
            
            List<Message> dto = new List<Message>();
            foreach (MessageEntity m in proj.Messages)
            {
                Message msg = new Message();
                msg.Content = m.Content;
                msg.SentTime = m.SentTime;
                msg.Sender = DocProjectUtils.ConstructName(accSvc.GetUserByEmail(m.Sender.Email));
                dto.Add(msg);
            }

            string serialized = JsonConvert.SerializeObject(dto);
            JsonResult result = Json(serialized, "application/json", JsonRequestBehavior.AllowGet);
            return result;
        }

        public JsonResult PushMessage(string projId, string chatMessage)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(1);

            MessageEntity msg = new MessageEntity();
            msg.Content = chatMessage;
            msg.Sender = GetUserByEmail(User.Identity.Name);
            msg.SentTime = DateTime.Now;

            DocProjectEntity project = FindProject(Convert.ToInt32(projId));
            project.Messages.Add(msg);

            container.SaveChanges();
            return Json(0);
        }
    }
}
