using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Utils;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class RequirementsController : Controller
    {
        //
        // GET: /Requirements/

        public ActionResult Index()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            //Full requirement viewing privileges
            if (Auth.GetCurrentUser().IsAdmin)
            {
                List<RequirementContext> requirements = new List<RequirementContext>();

                using (var db = new DataClassesDataContext())
                {
                    var result = (from r in db.Requirements
                                  where r.TenantId == Auth.GetCurrentUser().TenantId
                                  select r);
                    foreach (var requirement in result)
                    {
                        requirements.Add(new RequirementContext(requirement));
                    }
                }

                return View(requirements);
            }

            // Get all requirments for any projects manager owns
            else if (Auth.GetCurrentUser().IsManager)
            {
                List<RequirementContext> requirements = new List<RequirementContext>();
                using (var db = new DataClassesDataContext())
                {
                    // get all projects they own
                    var projects = (from p in db.Projects
                                    where p.ManagerId == Auth.GetCurrentUser().UserId
                                    && p.TenantId == Auth.GetCurrentUser().TenantId
                                    select p);

                    foreach (var project in projects)
                    {
                        // get all requirements for those projects
                        foreach (var projReq in project.ProjectRequirements)
                        {
                            requirements.Add(new RequirementContext(projReq.Requirement));
                        }
                    }                   
                }
                return View(requirements);
            }

            //Can only view requirements assigned to them
            else
            {
                List<RequirementContext> requirements = new List<RequirementContext>();

                using (var db = new DataClassesDataContext())
                {
                    var result = (from r in db.Requirements
                                  where r.TenantId == Auth.GetCurrentUser().TenantId &&
                                  r.AssignedUser == Auth.GetCurrentUser().UserId
                                  orderby r.Status ascending
                                  select r);
                    foreach (var requirement in result)
                    {
                        requirements.Add(new RequirementContext(requirement));
                    }
                }
                return View(requirements);
            }
        }

        //
        // GET: /Requirements/Create

        public ActionResult Create()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        //
        // POST: /Requirements/Create

        [HttpPost]
        public ActionResult Create(RequirementContext newRequirement)
        {            
            using (var db = new DataClassesDataContext())
            {
                User user = (from u in db.Users
                             where u.TenantId == Auth.GetCurrentUser().TenantId
                             && u.UserId == newRequirement.AssignedUserId
                             select u).FirstOrDefault();
                
                Requirement requirement = new Requirement();
                requirement.Description = !String.IsNullOrWhiteSpace(newRequirement.Description) ? newRequirement.Description : "";
                requirement.Status = newRequirement.StatusId != 0 ? newRequirement.StatusId : 1;    // Defaults to "Not Started"
                requirement.User = user;    // defaults to current user if none specified
                requirement.TypeId = newRequirement.TypeId;
                requirement.Time = newRequirement.Time;
                requirement.TenantId = Auth.GetCurrentUser().TenantId;

                db.Requirements.InsertOnSubmit(requirement);

                Project project = (from p in db.Projects
                             where p.ProjectId == newRequirement.ProjectId
                             && p.TenantId == Auth.GetCurrentUser().TenantId
                             select p).FirstOrDefault();

                ProjectRequirement pr = new ProjectRequirement()
                {
                    Project = project,
                    Requirement = requirement,
                    TenantId = project.TenantId
                };

                db.ProjectRequirements.InsertOnSubmit(pr);

                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    
    
        //
        // GET: /Requirements/Edit/{id}

        public ActionResult Edit(int id)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            RequirementContext requirementContext = null;

            using (var db = new DataClassesDataContext())
            {
                Requirement requirement = (from r in db.Requirements
                                   where r.TenantId == Auth.GetCurrentUser().TenantId && r.RequirementId == id
                                   select r).FirstOrDefault();
                if (requirement != null)
                {
                    requirementContext = new RequirementContext(requirement);
                }
            }

            if (requirementContext == null)
            {
                return View("NotFound");
            }

            return View(requirementContext);
        }

        //
        // POST: /Requirements/Edit
        
        [HttpPost]
        public ActionResult Edit(RequirementContext requirementToModify)
        {

            using (var db = new DataClassesDataContext())
            {
                var requirement = (from r in db.Requirements
                               where r.RequirementId == requirementToModify.RequirementId
                               select r).FirstOrDefault();
                
                //Check to make sure user actually input values

                if (!string.IsNullOrWhiteSpace(requirementToModify.Description) && requirement.Description != requirementToModify.Description) 
                    requirement.Description = requirementToModify.Description;
                
                if (requirement.Time != requirementToModify.Time && requirementToModify.Time != 0)
                    requirement.Time = requirementToModify.Time;
                
                if (requirementToModify.AssignedUser != null && requirement.AssignedUser != requirementToModify.AssignedUserId)
                    requirement.AssignedUser = requirementToModify.AssignedUserId;

                if (requirementToModify.StatusId > 0)
                    requirement.Status = requirementToModify.StatusId;

                if (requirementToModify.TypeId > 0)
                    requirement.TypeId = requirementToModify.TypeId;

                // if assigned to different project
                // remove any it was assigned to and assign to new one.
                // enforces 1->many relationship between projects and requirements
                if (requirementToModify.ProjectId != 0 && requirementToModify.ProjectId != requirement.ProjectRequirements.FirstOrDefault().ProjectId)
                {
                    requirement.ProjectRequirements.Clear();
                    requirement.ProjectRequirements.Add(new ProjectRequirement()
                    {
                        ProjectId = requirementToModify.ProjectId,
                        Requirement = requirement
                    });
                }

                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Requirements/UpdateStatus/{id}

        public ActionResult UpdateStatus(int id, int newStatus)
        {
            bool success = false;
            using (var db = new DataClassesDataContext())
            {
                Requirement req = db.Requirements.Where(r => r.TenantId == Auth.GetCurrentUser().TenantId && r.RequirementId == id).FirstOrDefault();
                if (req != null)
                {
                    req.Status = newStatus;
                    db.SubmitChanges();
                    success = true;
                }
            }

            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
    }
}
