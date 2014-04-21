using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;
using System.IO;
using System.Text.RegularExpressions;

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
            if (string.IsNullOrWhiteSpace(newTenant.OrgName) || string.IsNullOrWhiteSpace(newTenant.AdminUsername) ||
                string.IsNullOrWhiteSpace(newTenant.AdminPassword) || string.IsNullOrWhiteSpace(newTenant.ConfirmPassword))
            {
                return View();
            }

            if (newTenant.AdminPassword != newTenant.ConfirmPassword)
            {
                return View();
            }

            bool success = false;
            using (var db = new DataClassesDataContext())
            {
                var numMatched = db.Tenants.Where(t => t.OrgName == newTenant.OrgName).Count();
                numMatched += db.Users.Where(u => u.Username == newTenant.AdminUsername).Count();

                if (numMatched == 0)
                {
                    Tenant tenant = new Tenant();
                    tenant.OrgName = newTenant.OrgName;
                    tenant.LogoPath = "/Logos/logo1.jpg";
                    tenant.BannerColor = OnlyHexInString(newTenant.BannerColor) ? newTenant.BannerColor : "#357ebd";
                    tenant.TextColor = OnlyHexInString(newTenant.TextColor) ? newTenant.TextColor : "#FFFFFF";
                    tenant.CustomTypes = newTenant.CustomTypes;

                    User user = new User();
                    user.Username = newTenant.AdminUsername;
                    user.Password = Auth.GetPasswordHash(newTenant.AdminPassword);
                    user.RoleId = 1;

                    tenant.Users.Insert(tenant.Users.Count, user);

                    db.Tenants.InsertOnSubmit(tenant);
                    db.SubmitChanges();

                    if (file != null && file.ContentLength > 0)
                    {
                        // store the file inside ~/Logos/uploads folder. Name it Org Name of the tenant
                        string fileName = string.Format("{0}{1}", tenant.TenantId, Path.GetExtension(file.FileName));
                        string path = Path.Combine(Server.MapPath("~/Logos"), fileName);
                        file.SaveAs(path);
                        tenant.LogoPath = "/Logos/" + fileName;
                        db.SubmitChanges();
                    }

                    success = true;

                    Auth.Login(user);
                }
            }

            if (!success)
            {
                // need to add error message
                return RedirectToAction("Create");
            }

            return RedirectToAction("Index", "Admin");
        }

        public static bool OnlyHexInString(string id)
        {
            if (id == null)
            {
                return false;
            }
            
            return System.Text.RegularExpressions.Regex.IsMatch(id, @"#[0-9a-fA-F]{6}");
        }
    }
}
