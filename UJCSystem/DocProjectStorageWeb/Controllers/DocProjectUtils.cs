using DocProjectStorageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocProjectStorageWeb.Controllers
{
    public class DocProjectUtils
    {
        private static DocProjectModelContainer container = new DocProjectModelContainer();

        static public string ConstructName(DocProjectStorageWeb.AccountsServiceReference.User user)
        {
            return user.firstname + " " + user.lastname;
        }

        static public List<string> GetBaseRoles()
        {
            List<string> baseRoles = new List<string>();
            baseRoles.Add(DocProjectStorageWeb.WebModels.Role.REDACTOR.ToString());
            baseRoles.Add(DocProjectStorageWeb.WebModels.Role.TECHNICAL_REDACTOR.ToString());

            return baseRoles;
        }

        static public string ConstructRoleName(string baseRole, string docType)
        {
            return baseRole + "-" + docType;
        }

        static public bool HasUserRole(string userEmail, string roleName)
        {
            var users = from u in container.UserEntities
                        where u.Email == userEmail
                        select u;

            foreach (UserEntity user in users)
                foreach (UserRoleEntity role in user.Roles)
                {
                    if (role.RoleName == roleName)
                        return true;
                }

            return false;
        }

        static public List<DocumentType> GetAllDocumentTypes()
        {
            var types = from t in container.DocumentTypes
                        select t;


            List<DocumentType> list = types.ToList<DocumentType>();
            return list;
        }
    }
}