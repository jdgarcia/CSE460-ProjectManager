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

        public ActionResult Create()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(RequirementContext newRequirement)
        {
            string username = !String.IsNullOrWhiteSpace(newRequirement.AssignedUser) ? newRequirement.AssignedUser : Auth.GetCurrentUser().Username;
            
            using (var db = new DataClassesDataContext())
            {
                User user = (from u in db.Users
                             where u.TenantId == Auth.GetCurrentUser().TenantId
                             && username == u.Username
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
                             && p.TenantId == newRequirement.TenantId
                             select p).FirstOrDefault();

                ProjectRequirement pr = new ProjectRequirement()
                {
                    Project = project,
                    Requirement = requirement
                };

                db.ProjectRequirements.InsertOnSubmit(pr);

                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
