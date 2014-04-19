using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        //
        // GET: /Projects/

        public ActionResult Index()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            return View(Auth.GetCurrentUser());
        }

        //
        // GET: /Projects/Create

        public ActionResult Create()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }


            return View();
        }

        //
        // GET: /Projects/Edit/{id}

        public ActionResult Edit(int id)
        {
            // this function only returns the page for editing a project
            // we'll need a seperate Edit(Project project) for actually editing a project

            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            DataClassesDataContext db = new DataClassesDataContext();
            Project project = (from p in db.Projects
                               where p.ProjectId == id
                               select p).FirstOrDefault();
            // still need to match TenantId once we implement logins

            if (project == null)
            {
                return View("NotFound");
            }

            return View(project);
        }
    }
}
