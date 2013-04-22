using DocProjectStorageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocProjectStorageWeb.Controllers
{
    public class SetupController : Controller
    {
        private static DocProjectModelContainer container = new DocProjectModelContainer();

        public ActionResult Index()
        {
             DocProjectStorageWeb.WebModels.SetupModel model = new DocProjectStorageWeb.WebModels.SetupModel();
            model.DocumentTypeNames = new List<string>();
            var types = from p in container.DocumentTypes
                          select p;

            List<DocumentType> docTypesList = types.ToList<DocumentType>();
            foreach (DocumentType type in docTypesList)
            {
                model.DocumentTypeNames.Add(type.Name);
            }
            
            return View(model);
        }

        public ActionResult UserRoles(string user)
        {
            DocProjectStorageWeb.WebModels.UserRolesModel model = new DocProjectStorageWeb.WebModels.UserRolesModel();
            model.UserName = user;

            model.Roles = new List<DocProjectStorageWeb.WebModels.RoleSelection>();

            List<string> rolesList = GetRolesList();
            foreach (string roleName in rolesList)
            {
                DocProjectStorageWeb.WebModels.RoleSelection role = new DocProjectStorageWeb.WebModels.RoleSelection();
                role.RoleName = roleName;
                role.IsSet = DocProjectUtils.HasUserRole(user,roleName);

                model.Roles.Add(role);
            }


            return View("UserRoles", model);
        }

        [Authorize]
        public ActionResult AddDocType(string typeName)
        {
            var docTypes = from u in container.DocumentTypes
                        where u.Name == typeName
                        select u;

            if (docTypes.Count<DocumentType>() != 0)
            {
                return RedirectToAction("Index");
            }

            DocumentType newType = new DocumentType();
            newType.Name = typeName;

            container.DocumentTypes.Add(newType);
            container.SaveChanges();

            return RedirectToAction("Index");
        }

        private List<string> GetRolesList()
        {
            var types = from p in container.DocumentTypes
                          select p;
            
            List<DocumentType> docTypes = types.ToList<DocumentType>();
            List<string> docTypesList = new List<string>();
            foreach (DocumentType type in docTypes)
            {
                docTypesList.Add(type.Name); 
            }


            List<String> baseRoles = DocProjectUtils.GetBaseRoles();


            List<string> roles = new List<string>();
            roles.Add("Admin");


            foreach (string baseRole in baseRoles)
                foreach (string docType in docTypesList)
                    roles.Add(DocProjectUtils.ConstructRoleName(baseRole, docType));
            
            //roles.Add(


            return roles;
        }

        

        
    }
}
