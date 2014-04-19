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
        public ActionResult Create(NewTenant newTenant)
        {
            // TODO: check if OrgName already exists
            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = new Tenant();
                tenant.OrgName = newTenant.OrgName;

                Admin admin = new Admin();
                admin.Username = newTenant.AdminUserName;
                admin.Password = newTenant.AdminPassword;
                // TODO: encrypt passwords

                tenant.Admins.Insert(tenant.Admins.Count, admin);

                db.Tenants.InsertOnSubmit(tenant);
                db.SubmitChanges();
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}
