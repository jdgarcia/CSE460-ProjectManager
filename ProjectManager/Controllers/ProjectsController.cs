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
                // employees just view their assigned requirements, not list of projects
                return RedirectToAction("Index", "Requirements");

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
                        //Filter by project name, status, and project manager 
                        if (project.Name.ToLowerInvariant().Contains(Filter) ||
                            project.Status1.Name.ToLowerInvariant().Contains(Filter) ||
                            project.User.Username.ToLowerInvariant().Contains(Filter))
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
            project.Name = (!string.IsNullOrWhiteSpace(newProject.Name)) ? newProject.Name : "(Untitled Project)";
            project.Start = (newProject.Start != null) ? newProject.Start : DateTime.Now.Date;
            project.ExpectedEnd = (newProject.ExpectedEnd != null) ? newProject.ExpectedEnd : DateTime.Now.AddMonths(1);
            project.Status = 1;

            //Check if the user put the start date after the end date, and swap if so
            if (project.Start.Value.CompareTo(project.ExpectedEnd.Value) > 0)
            {
                var start = project.Start;
                project.Start = project.ExpectedEnd;
                project.ExpectedEnd = start;
            }

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
            if (!Auth.IsLoggedIn() || !Auth.GetCurrentUser().IsManager)
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

                project.Name = (!string.IsNullOrWhiteSpace(projectToModify.Name)) ? projectToModify.Name : "(Untitled Project)";
                if (projectToModify.RawDateStart != null && projectToModify.RawDateStart != DateTime.MinValue)
                    project.Start = projectToModify.RawDateStart;
                if (projectToModify.RawDateEnd != null && projectToModify.RawDateEnd != DateTime.MinValue)
                    project.ExpectedEnd = projectToModify.RawDateEnd;

                //Check if the start date occurs after the end date, and swap them if so
                if (project.Start.Value.CompareTo(project.ExpectedEnd.Value) > 0)
                {
                    var start = project.Start;
                    project.Start = project.ExpectedEnd;
                    project.ExpectedEnd = start;
                }

                if (projectToModify.StatusId > 0)
                    project.Status = projectToModify.StatusId;

                db.SubmitChanges();

            }
            return RedirectToAction("Index");
        }

    }
}
