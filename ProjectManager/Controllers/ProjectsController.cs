using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        //
        // GET: /Project/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Project/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // GET: /Project/Edit/{id}

        public ActionResult Edit(int id)
        {
            // this function only returns the page for editing a project
            // we'll need a seperate Edit(Project project) for actually editing a project

            DataClassesDataContext db = new DataClassesDataContext();
            Project project = (from p in db.Projects
                               where p.ProjectId == id
                               select p).FirstOrDefault();
            // still need to match TenantId once we implement logins

            if (project == null)
            {
                return RedirectToAction("NotFound");
            }

            return View(project);
        }

        //
        // GET: /Project/NotFound

        public ActionResult NotFound()
        {
            return View();
        }
    }
}
