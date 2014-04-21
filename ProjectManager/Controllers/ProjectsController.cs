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
            //Full project viewing privileges
            if (Auth.GetCurrentUser().IsAdmin)
            {
                List<ProjectContext> projects = new List<ProjectContext>();

                using (var db = new DataClassesDataContext())
                {
                    var result = (from p in db.Projects
                                  where p.TenantId == Auth.GetCurrentUser().TenantId
                                  orderby p.ExpectedEnd ascending
                                  select p);
                    foreach (var project in result)
                    {
                        projects.Add(new ProjectContext(project));
                    }
                }

                return View(projects);
            }
            //Can only view projects they manage
            else if (Auth.GetCurrentUser().IsManager)
            {
                List<ProjectContext> projects = new List<ProjectContext>();

                using (var db = new DataClassesDataContext())
                {
                    var result = (from p in db.Projects
                                  where p.TenantId == Auth.GetCurrentUser().TenantId &&
                                  p.ManagerId == Auth.GetCurrentUser().UserId
                                  orderby p.ExpectedEnd ascending
                                  select p);
                    foreach (var project in result)
                    {
                        projects.Add(new ProjectContext(project));
                    }
                }
                return View(projects);
            }
            else
                //Will be replaced with viewing requirements, once those are available
                return View("NotFound");

        }

        //
        // POST: /Project/
        [HttpPost]
        public ActionResult Index(string Filter)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            //Full project viewing privileges
            if (Auth.GetCurrentUser().IsAdmin)
            {
                List<ProjectContext> projects = new List<ProjectContext>();

                using (var db = new DataClassesDataContext())
                {
                    var result = (from p in db.Projects
                                  where p.TenantId == Auth.GetCurrentUser().TenantId
                                  orderby p.ExpectedEnd ascending
                                  select p);
                    foreach (var project in result)
                    {
                        if (project.Name.ToLowerInvariant().Contains(Filter) ||
                            project.Status1.Name.ToLowerInvariant().Contains(Filter))
                            projects.Add(new ProjectContext(project));
                    }
                }

                return View(projects);
            }
            //Can only view projects they manage
            else if (Auth.GetCurrentUser().IsManager)
            {
                List<ProjectContext> projects = new List<ProjectContext>();

                using (var db = new DataClassesDataContext())
                {
                    var result = (from p in db.Projects
                                  where p.TenantId == Auth.GetCurrentUser().TenantId &&
                                  p.ManagerId == Auth.GetCurrentUser().UserId
                                  orderby p.ExpectedEnd ascending
                                  select p);
                    foreach (var project in result)
                    {
                        if (project.Name.ToLowerInvariant().Contains(Filter) ||
                            project.Status1.Name.ToLowerInvariant().Contains(Filter))
                        projects.Add(new ProjectContext(project));
                    }
                }
                return View(projects);
            }
            else
                //Will be replaced with viewing requirements, once those are available
                return View("NotFound");
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
            Project project = new Project();
            project.TenantId = Auth.GetCurrentUser().TenantId;
            project.ManagerId = Auth.GetCurrentUser().UserId;
            project.Name = newProject.Name;

            //Check if the user put the start date after the end date, and swap if so
            if (newProject.Start.Value.CompareTo(newProject.ExpectedEnd.Value) > 0)
            {
                project.Start = newProject.ExpectedEnd;
                project.ExpectedEnd = newProject.Start;
            }
            else
            {
                project.Start = newProject.Start;
                project.ExpectedEnd = newProject.ExpectedEnd;
            }
            project.Status = 1;
            using (var db = new DataClassesDataContext())
            {
                db.Projects.InsertOnSubmit(project);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Projects/Details/{id}

        public ActionResult Details(int id)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            ProjectContext projectContext = null;

            using (var db = new DataClassesDataContext())
            {
                Project project = (from p in db.Projects
                                   where p.TenantId == Auth.GetCurrentUser().TenantId && p.ProjectId == id
                                   select p).FirstOrDefault();
                if (project != null)
                {
                    projectContext = new ProjectContext(project);
                }
            }

            if (projectContext == null)
            {
                return View("NotFound");
            }

            return View(projectContext);
        }

        //
        // GET: /Projects/Edit/{id}

        public ActionResult Edit(int id)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            ProjectContext projectContext = null;

            using (var db = new DataClassesDataContext())
            {
                Project project = (from p in db.Projects
                                   where p.TenantId == Auth.GetCurrentUser().TenantId && p.ProjectId == id
                                   select p).FirstOrDefault();
                if (project != null)
                {
                    projectContext = new ProjectContext(project);
                }
            }

            if (projectContext == null)
            {
                return View("NotFound");
            }

            return View(projectContext);
        }

        //
        // POST: /Projects/Edit
        
        [HttpPost]
        public ActionResult Edit(ProjectContext projectToModify)
        {

            using (var db = new DataClassesDataContext())
            {
                var project = (from p in db.Projects
                               where p.ProjectId == projectToModify.ProjectId
                               select p).FirstOrDefault();
                
                //Check to make sure user actually input values
                if (projectToModify.Name != null)
                    project.Name = projectToModify.Name;

                //Check if the start date occurs after the end date, and swap them if so
                if (projectToModify.RawDateStart.CompareTo(projectToModify.RawDateEnd) > 0)
                {
                    project.Start = projectToModify.RawDateEnd;
                    project.ExpectedEnd = projectToModify.RawDateStart;
                }
                else
                {
                    project.Start = projectToModify.RawDateStart;
                    project.ExpectedEnd = projectToModify.RawDateEnd;
                }

                if (projectToModify.StatusId > 0)
                    project.Status = projectToModify.StatusId;

                project.ManagerId = projectToModify.ManagerId;

                //project.ManagerId = projectToModify.ManagerId;
                db.SubmitChanges();

            }
            return RedirectToAction("Index");
        }

    }
}
