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

            // file should replace old one with same name so no update to db necessary
            // but to avoid having to manually change db and commit, i'll reset it anyway :P
            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = (from t in db.Tenants
                              where t.TenantId == Auth.GetCurrentUser().TenantId
                              select t).FirstOrDefault();

                string fileName = string.Format("{0}{1}", tenant.TenantId, Path.GetExtension(file.FileName));
                string path = Path.Combine(Server.MapPath("~/Logos"), fileName);
                file.SaveAs(path);

                tenant.LogoPath = "/Logos/" + fileName;
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Admin/ChangeColors

        [HttpPost]
        public ActionResult ChangeColors(string bannerColor, string textColor)
        {
            if (!Auth.IsLoggedIn() || !Auth.GetCurrentUser().IsAdmin)
            {
                return RedirectToAction("Login", "Home");
            }

            bool changeBanner = !String.IsNullOrWhiteSpace(bannerColor) && bannerColor != Auth.GetCurrentUser().BannerColor;
            bool changeText = !String.IsNullOrWhiteSpace(textColor) && bannerColor != Auth.GetCurrentUser().TextColor;

            if (!changeBanner && !changeText)
            {
                return RedirectToAction("Index");
            }

            if (changeBanner && !TenantController.OnlyHexInString(bannerColor)) 
            {
                return View("Error with color");
            }
            else if (changeText && !TenantController.OnlyHexInString(textColor))
            {
                return View("Error with color");
            }

            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = (from t in db.Tenants
                                 where t.TenantId == Auth.GetCurrentUser().TenantId
                                 select t).FirstOrDefault();

                if (changeBanner) tenant.BannerColor = bannerColor;
                if (changeText) tenant.TextColor = textColor;

                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
