using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
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

        //
        // POST: /Admin/EditLogo

        [HttpPost]
        public ActionResult EditLogo(HttpPostedFileBase file)
        {
            if (!Auth.IsLoggedIn() || !Auth.GetCurrentUser().IsAdmin) 
            {
                return RedirectToAction("Login", "Home");    
            }

            if (file == null || file.ContentLength <= 0)
            {
                // No file specified
                return View();
            }

            var ext = Path.GetExtension(file.FileName);
            string fileName = Regex.Replace(Auth.GetCurrentUser().TenantName + ext, @"\s", "_");

            // store the file inside ~/Logos/uploads folder
            var path = Path.Combine(Server.MapPath("~/Logos"), fileName);
            file.SaveAs(path);

            // file should replace old one with same name so no update to db necessary

            // but to avoid having to manually change db and commit, i'll reset it anyway :P
            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = (from t in db.Tenants
                              where t.TenantId == Auth.GetCurrentUser().TenantId
                              select t).FirstOrDefault();

                tenant.LogoPath = "/Logos/"+fileName;

                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
