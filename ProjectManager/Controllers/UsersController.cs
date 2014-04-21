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
            if (!Auth.GetCurrentUser().IsAdmin)
            {
                return View("AccessDenied");
            }

            List<UserContext> users = new List<UserContext>();

            using (var db = new DataClassesDataContext())
            {
                var result = (from u in db.Users
                              where u.TenantId == Auth.GetCurrentUser().TenantId
                              orderby u.RoleId, u.Username
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
            if (!Auth.GetCurrentUser().IsAdmin)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // POST: /Users/Create
        
        [HttpPost]
        public ActionResult Create(User newUser)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!Auth.GetCurrentUser().IsAdmin)
            {
                return RedirectToAction("Index");
            }

            using (var db = new DataClassesDataContext())
            {
                var existingUser = (from u in db.Users
                                    where u.Username == newUser.Username
                                    select u).FirstOrDefault();

                if (existingUser == null)
                {
                    User user = new User();
                    user.TenantId = Auth.GetCurrentUser().TenantId;
                    user.Username = newUser.Username;
                    user.Password = Auth.GetPasswordHash(newUser.Password);
                    user.RoleId = 5;    // default to GLOBAL.Employee for now

                    db.Users.InsertOnSubmit(user);
                    db.SubmitChanges();
                }
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Users/Edit/{id}

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!Auth.GetCurrentUser().IsAdmin)
            {
                return RedirectToAction("Index");
            }

            DataClassesDataContext db = new DataClassesDataContext();
            User user = (from u in db.Users
                         where u.UserId == id && u.TenantId == Auth.GetCurrentUser().TenantId
                         select u).FirstOrDefault();

            if (id == Auth.GetCurrentUser().UserId)
                return View("SelfEditError");
            if (user == null)
                return View("NotFound");

            return View(user);
        }

        //
        // POST: /Users/Edit
        [HttpPost]
        public ActionResult Edit(User userToModify)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!Auth.GetCurrentUser().IsAdmin)
            {
                return RedirectToAction("Index");
            }

            using (var db = new DataClassesDataContext())
            {
                var user = (from u in db.Users
                            where u.UserId == userToModify.UserId
                            select u).FirstOrDefault();

                if (userToModify.Username != null)
                    user.Username = userToModify.Username;
                if (userToModify.RoleId > 0)
                    user.RoleId = userToModify.RoleId;

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
                Debug.WriteLine("You Fail!!!"); //Harsh
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
