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

            List<ProjectContext> projects = new List<ProjectContext>();

            using (var db = new DataClassesDataContext())
            {
                var result = (from p in db.Projects
                              where p.TenantId == Auth.GetCurrentUser().TenantId
                              select p);
                foreach (var project in result)
                {
                    projects.Add(new ProjectContext(project));
                }
            }

            return View(projects);
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
        // POST: /Projects/Create

        [HttpPost]
        public ActionResult Create(Project newProject)
        {
            // create new project here
            // you will need a form in Views/Projects/Create
            // the 'name' of each input should be the same as the required Project fields
            // ex: <input type="text" name="Name"> would be the input for Project.Name
            Project project = new Project();
            project.TenantId = Auth.GetCurrentUser().TenantId;
            project.ManagerId = Auth.GetCurrentUser().UserId;
            project.Name = newProject.Name;
            project.Start = newProject.Start;
            project.ExpectedEnd = newProject.ExpectedEnd;
            project.Status = 1;
            using (var db = new DataClassesDataContext())
            {
                db.Projects.InsertOnSubmit(project);
                db.SubmitChanges();

            }

            // return user to Projects/Index after creation is done
            return RedirectToAction("Index");
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
        
        [HttpPost]
        public ActionResult Edit(Project projectToModify)
        {

            using (var db = new DataClassesDataContext())
            {
                var project = (from p in db.Projects
                               where p.ProjectId == projectToModify.ProjectId
                               select p).FirstOrDefault();
                
                //Check to make sure user actually input values
                if (projectToModify.Name != null)
                    project.Name = projectToModify.Name;
                if (projectToModify.Start != null)
                    project.Start = projectToModify.Start;
                if (projectToModify.ExpectedEnd != null)
                    project.ExpectedEnd = projectToModify.ExpectedEnd;

                if (projectToModify.Status > 0)
                    project.Status = projectToModify.Status;

                //project.ManagerId = projectToModify.ManagerId;
                db.SubmitChanges();

            }
            return RedirectToAction("Index");
        }

    }
}
