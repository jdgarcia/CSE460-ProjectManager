using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public string Test()
        {
            DataClassesDataContext db = new DataClassesDataContext();

            Tenant test = (from t in db.Tenants
                         select t).FirstOrDefault();

            db.Dispose();
            return test.OrgName;
        }
    }
}
