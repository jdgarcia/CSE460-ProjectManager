﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;

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
                user.Username = newTenant.AdminUsername;
                user.Password = Auth.GetPasswordHash(newTenant.AdminPassword);
                user.Role = "Admin";
                // TODO: encrypt passwords

                Admin admin = new Admin();
                admin.Username = newTenant.AdminUsername;
                admin.Password = newTenant.AdminPassword;

                tenant.Users.Insert(tenant.Users.Count, user);
                tenant.Admins.Insert(tenant.Admins.Count, admin);

                db.Tenants.InsertOnSubmit(tenant);
                db.SubmitChanges();

                Auth.Login(user);
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}
