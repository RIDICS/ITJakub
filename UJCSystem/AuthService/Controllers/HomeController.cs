using AuthService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuthService.Controllers
{
    public class HomeController : Controller
    {

        private IAccountsService accountsService = new AccountsService();

        private static readonly string SESSION_USERNAME = "SESSION_USERNAME_KEY";
        private string userName;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AuthModel model)
        {
            if (ModelState.IsValid)
            {
                User user = accountsService.GetUserByEmail(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Zadaný e-mailem není registrován");
                    return View("Index", model);
                }
                if (!accountsService.Authenticate(model.Email, model.Password))
                {
                    ModelState.AddModelError("", "Nesprávné heslo");
                    return View("Index", model);
                }
                if (!accountsService.HasRole(user, "admin"))
                {
                    ModelState.AddModelError("", "Uživatel nemá práva admina");
                    return View("Index", model);
                }

                Session[SESSION_USERNAME] = model.Email;

                return RedirectToAction("AdminPage", "Home");
            }
            else
            {
                return View("Index", model);
            }
        }

        public ActionResult AdminPage()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            userName = accountsService.GetUserNameByEmail(Session[SESSION_USERNAME].ToString());
            ViewBag.UserName = userName;
            ViewBag.Users = accountsService.GetAllUsers();

            return View();
        }

        public ActionResult RegisterPage()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            RegisterModel model = new RegisterModel();
            model.Roles = InitRoles();
            return View(model);
        }

        private bool IsAdmin()
        {
            Object objUserName = Session[SESSION_USERNAME];
            if (objUserName == null)
            { // user not logged in
                return false;
            }
            return true;
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("RegisterPage", model);
            }

            User user = new User();
            user.email = model.Email;
            user.firstname = model.FirstName;
            user.lastname = model.LastName;
            user.pwhash = SecurityUtils.HashPassword(user.email, user.pwhash);
            user.role = model.Role;
            accountsService.AddUser(user);

            return RedirectToAction("AdminPage", "Home");
        }

        public ActionResult EditPage(int id)
        {
            User user = accountsService.GetUserById(id);

            EditModel model = new EditModel();
            model.Id = id;
            model.Roles = InitRoles();
            model.Role = user.role;
            model.FirstName = user.firstname;
            model.LastName = user.lastname;

            return View(model);
        }

        public ActionResult Edit(EditModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            User user = accountsService.GetUserById(model.Id);

            if (user == null)
            {
                // TODO never happens?
            }
            user.firstname = model.FirstName;
            user.lastname = model.LastName;
            user.role = model.Role;

            if (model.Password != null && model.Password != "")
            {
                string pw = SecurityUtils.HashPassword(user.email, model.Password);
                user.pwhash = pw;
            }
            accountsService.SaveContextChanges();
            return RedirectToAction("AdminPage", "Home");
        }

        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }


            User user = accountsService.GetUserById(id);
            if (userName == user.email)
            {
                Session[SESSION_USERNAME] = null;
            }
            accountsService.RemoveUser(user);
            return RedirectToAction("AdminPage", "Home");
        }

        private IEnumerable<string> InitRoles()
        {
            List<string> list = new List<string>();
            list.Add("user");
            list.Add("admin");
            return list;
        }
    }
}
