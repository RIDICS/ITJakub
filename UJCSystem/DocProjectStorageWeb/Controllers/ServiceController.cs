using DocProjectStorageWeb.Models;
using DocProjectStorageWeb.WebModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocProjectStorageWeb.Controllers
{
    public class ServiceController : Controller
    {
        private static DocProjectModelContainer container = new DocProjectModelContainer();

        public JsonResult FetchUsers(string term)
        {
            var users = from u in container.UserEntities
                        where u.Email.Contains(term)
                        select u;

            List<UserEntity> usersList = users.ToList<UserEntity>();

            List<UserModel> usersNames = new List<UserModel>();
            foreach (UserEntity userEntity in usersList)
            {
                UserModel user = new UserModel();
                user.Name = userEntity.Email;
                usersNames.Add(user);
            }

            string serialized = JsonConvert.SerializeObject(usersNames);
            JsonResult result = Json(serialized, "application/json", JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}
