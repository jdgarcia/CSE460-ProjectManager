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
        // GET: /Tenant/

        [HttpPost]
        public ActionResult Create(string OrgName)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            Tenant newTenant = new Tenant();
            newTenant.OrgName = OrgName;
            db.Tenants.InsertOnSubmit(newTenant);
            db.SubmitChanges();
            db.Dispose();

            return RedirectToAction("Index", "Home");
        }

    }
}
