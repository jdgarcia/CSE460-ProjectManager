using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;

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
        // GET: /Projects/Create

        public ActionResult Create()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        //
        // POST: /Users/
        [HttpPost]

        public ActionResult Create(User newUser)
        {
            if (!Auth.GetCurrentUser().IsAdmin)
            {
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
    }
}
