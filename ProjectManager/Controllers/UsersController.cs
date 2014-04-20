using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;
using System.Diagnostics;

namespace ProjectManager.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Users/

        public ActionResult Index()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            List<UserContext> users = new List<UserContext>();

            using (var db = new DataClassesDataContext())
            {
                var result = (from u in db.Users
                              where u.TenantId == Auth.GetCurrentUser().TenantId
                              select u);
                foreach (var user in result)
                {
                    users.Add(new UserContext(user));
                }
            }

            return View(users);
        }

        //
        // GET: /Users/Create

        public ActionResult Create()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        //
        // POST: /Users/Create
        
        [HttpPost]
        public ActionResult Create(User newUser)
        {
            if (!Auth.GetCurrentUser().IsAdmin)
            {
                // TODO: need to add proper error message handling/display
                return View("You do not have permisions to do that");
            }
            
            using (var db = new DataClassesDataContext())
            {
                User user = new User();
                user.TenantId = Auth.GetCurrentUser().TenantId;
                user.Username = newUser.Username;
                user.Password = Auth.GetPasswordHash(newUser.Password);
                user.RoleId = 5;    // default to GLOBAL.Employee for now

                db.Users.InsertOnSubmit(user);
                db.SubmitChanges();

            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Users/ChangePass

        public ActionResult ChangePass()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        //
        // Put: /Users/ChangePass

        [HttpPost]
        public ActionResult ChangePass(string oldPass, string newPass)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            // attempt to log in with current username and oldpass
            LoginContext tryLogin = new LoginContext();
            tryLogin.Username = Auth.GetCurrentUser().Username;
            tryLogin.Password = oldPass;

            if (!Auth.Login(tryLogin))
            {
                // FAILED!!! Need to set error code
                Debug.WriteLine("You Fail!!!");
                return View();
            }


            // update newPass in database
            using (var db = new DataClassesDataContext())
            {
                User user = (from u in db.Users
                              where u.TenantId == Auth.GetCurrentUser().TenantId
                                && u.UserId == Auth.GetCurrentUser().UserId
                              select u).FirstOrDefault();

                user.Password = Auth.GetPasswordHash(newPass);

                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
