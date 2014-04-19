using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;

namespace ProjectManager.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (Auth.IsLoggedIn())
            {
                return RedirectToAction("Index", "Projects");
            }

            return View();
        }

        //
        // GET: /Home/Login/

        public ActionResult Login()
        {
            if (Auth.IsLoggedIn())
            {
                return RedirectToAction("Index", "Projects");
            }

            // TODO: add return url feature, or just an Authorize attribute
            return View();
        }

        //
        // POST: /Home/Login

        [HttpPost]
        public ActionResult Login(LoginContext userInfo)
        {
            if (Auth.Login(userInfo))
            {
                return RedirectToAction("Index", "Projects");
            }

            return RedirectToAction("Login");
        }

        //
        // GET: /Home/Logout

        public ActionResult Logout()
        {
            Auth.Logout();

            return RedirectToAction("Index");
        }

        //
        // GET: /Home/Test

        public string Test()
        {
            DataClassesDataContext db = new DataClassesDataContext();

            Tenant test = (from t in db.Tenants
                         select t).FirstOrDefault();

            db.Dispose();
            return test.OrgName;
        }
    }
}
