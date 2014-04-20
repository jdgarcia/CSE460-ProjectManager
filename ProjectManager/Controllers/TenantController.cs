using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;
using System.IO;

namespace ProjectManager.Controllers
{
    public class TenantController : Controller
    {
        //
        // GET: /Tenant/Create/

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Tenant/Create/{newTenant}

        [HttpPost]
        public ActionResult Create(NewTenantContext newTenant, HttpPostedFileBase file)
        {
            if (string.IsNullOrEmpty(newTenant.OrgName) || string.IsNullOrEmpty(newTenant.AdminUsername) || string.IsNullOrEmpty(newTenant.AdminPassword))
            {
                return View();
            }

            
            var fileName = "";
            if (file != null && file.ContentLength > 0)
            {
                // extract only the fielname
                fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/Logos/uploads folder
                var path = Path.Combine(Server.MapPath("~/Logos"), fileName);
                file.SaveAs(path);
            }

            // TODO: check if OrgName already exists
            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = new Tenant();
                tenant.OrgName = newTenant.OrgName;
                tenant.LogoPath = "/Logos/"+fileName;
                tenant.BannerColor = OnlyHexInString(newTenant.BannerColor) ? newTenant.BannerColor : "#357ebd";
                tenant.TextColor = OnlyHexInString(newTenant.TextColor) ? newTenant.TextColor : "#FFFFFF";


                User user = new User();
                user.Username = newTenant.AdminUsername;
                user.Password = Auth.GetPasswordHash(newTenant.AdminPassword);
                user.RoleId = 1;

                tenant.Users.Insert(tenant.Users.Count, user);

                db.Tenants.InsertOnSubmit(tenant);
                db.SubmitChanges();

                Auth.Login(user);
            }

            return RedirectToAction("Index", "Admin");
        }

        //
        // GET: /Tenant/OnlyHexInString/{color}

        public bool OnlyHexInString(string id)
        {
            if (id == null)
            {
                return false;
            }
            
            return System.Text.RegularExpressions.Regex.IsMatch(id, @"#[0-9a-fA-F]{6}");
        }
    }
}
