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
            HttpCookie userCookie = Request.Cookies["ProjectManagerUserSession"];
            if (userCookie != null)
            {
                Auth.Login(new LoginContext { Username = userCookie["username"], Password = userCookie["password"] });
            }
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
                SaveUserCookie(userInfo);
                return RedirectToAction("Index", "Projects");
            }

            return RedirectToAction("Login");
        }

        //
        // GET: /Home/Logout

        public ActionResult Logout()
        {
            Auth.Logout();
            DeleteUserCookie();

            return RedirectToAction("Index");
        }

        //
        // GET: /Home/Test

        public string Test()
        {
            DataClassesDataContext db = new DataClassesDataContext();

            Tenant test = (from t in db.Tenants
                           where t.TenantId == 1
                           select t).FirstOrDefault();

            db.Dispose();
            return test.OrgName;
        }

        //
        // GET: /Home/PassGen/{pass}

        public string PassGen(string id)
        {
            return Auth.GetPasswordHash(id);
        }

        private void SaveUserCookie(LoginContext userInfo)
        {
            HttpCookie userCookie = new HttpCookie("ProjectManagerUserSession");
            userCookie["username"] = userInfo.Username;
            userCookie["password"] = userInfo.Password;
            userCookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Add(userCookie);
        }

        private void DeleteUserCookie()
        {
            HttpCookie userCookie = Request.Cookies["ProjectManagerUserSession"];
            if (userCookie != null)
            {
                userCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(userCookie);
            }
        }
    }
}
