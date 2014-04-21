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

    }
}
