using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;

namespace ProjectManager.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index() // Displays tenant specific settings
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (!Auth.GetCurrentUser().IsAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(Auth.GetCurrentUser());
        }
    }
}
