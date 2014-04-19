using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

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
        public ActionResult Create(NewTenantContext newTenant)
        {
            // TODO: check if OrgName already exists
            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = new Tenant();
                tenant.OrgName = newTenant.OrgName;

                User user = new User();
                user.Username = newTenant.AdminUserName;
                user.Password = newTenant.AdminPassword;
                user.Role = "Admin";
                // TODO: encrypt passwords

                Admin admin = new Admin();
                admin.Username = newTenant.AdminUserName;
                admin.Password = newTenant.AdminPassword;

                tenant.Users.Insert(tenant.Users.Count, user);
                tenant.Admins.Insert(tenant.Admins.Count, admin);

                db.Tenants.InsertOnSubmit(tenant);
                db.SubmitChanges();

                System.Web.HttpContext.Current.Session["CurrentUser"] = user;
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}
