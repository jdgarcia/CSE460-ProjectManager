using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Utils
{
    public class DbUtils
    {
        public static Requirement GetRequirementById(int id)
        {
            Requirement target = null;
            using (var db = new DataClassesDataContext())
            {
                target = db.Requirements.Where(r => r.TenantId == Auth.GetTenantId() && r.RequirementId == id).FirstOrDefault();
            }

            return target;
        }

        public static RequirementType GetRequirementTypeById(int id)
        {
            RequirementType target = null;
            using (var db = new DataClassesDataContext())
            {
                target = db.RequirementTypes.Where(t => t.TenantId == Auth.GetTenantId() && t.TypeId == id).FirstOrDefault();
            }

            return target;
        }

        public static List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>();

            using (var db = new DataClassesDataContext())
            {
                projects = db.Projects.Where(p => p.TenantId == Auth.GetTenantId()).ToList();
            }

            return projects;
        }

        public static List<RequirementType> GetCustomTypes()
        {
            List<RequirementType> types = new List<RequirementType>();

            if (Auth.IsUsingCustomTypes())
            {
                using (var db = new DataClassesDataContext())
                {
                    types = db.RequirementTypes.Where(t => t.TenantId == Auth.GetTenantId()).ToList();
                }
            }

            return types;
        }

        public static List<ProjectContext> GetProjectsByRequirementType(int rTypeId)
        {
            List<ProjectContext> projects = new List<ProjectContext>();

            using (var db = new DataClassesDataContext())
            {
                var matchedProjects = (from r in db.Requirements
                                        join pr in db.ProjectRequirements on r.RequirementId equals pr.RequirementId
                                        join p in db.Projects on pr.ProjectId equals p.ProjectId
                                        where r.RequirementType.TypeId == rTypeId
                                        select p);
                foreach (Project project in matchedProjects)
                {
                    projects.Add(new ProjectContext(project));
                }
            }

            return projects;
        }

        public static List<SelectListItem> GetRoleSelectItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new DataClassesDataContext())
            {
                var roles = db.Roles.Where(r => r.TenantId == 1 || r.TenantId == Auth.GetCurrentUser().TenantId);
                foreach (Role role in roles)
                {
                    items.Add(new SelectListItem { Value = role.RoleId.ToString(), Text = role.Title });
                }
            }

            return items;
        }

        public static List<SelectListItem> GetStatusSelectItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new DataClassesDataContext())
            {
                var statuses = db.Status.Where(s => s.TenantId == 1 || s.TenantId == Auth.GetCurrentUser().TenantId);
                foreach (Status status in statuses)
                {
                    items.Add(new SelectListItem { Value = status.StatusId.ToString(), Text = status.Name });
                }
            }

            return items;
        }

        public static List<SelectListItem> GetRequirementTypeSelectItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new DataClassesDataContext())
            {
                var types = db.RequirementTypes.Where(t => t.TenantId == 1 || t.TenantId == Auth.GetCurrentUser().TenantId).OrderBy(t => t.Name);
                foreach (RequirementType type in types)
                {
                    items.Add(new SelectListItem { Value = type.TypeId.ToString(), Text = type.Name });
                }
            }

            return items;
        }

        public static List<SelectListItem> GetUserSelectItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new DataClassesDataContext())
            {
                var users = db.Users.Where(u => u.TenantId == Auth.GetCurrentUser().TenantId && u.RoleId == 5).OrderBy(u => u.Username);
                foreach (User user in users)
                {
                    items.Add(new SelectListItem { Value = user.UserId.ToString(), Text = user.Username });
                }
            }

            return items;
        }

        public static List<SelectListItem> GetManagedProjectSelectItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (var db = new DataClassesDataContext())
            {
                var currentUser = Auth.GetCurrentUser();
                var projects = db.Projects.Where(p => p.TenantId == currentUser.TenantId && p.ManagerId == currentUser.UserId);
                foreach (Project project in projects)
                {
                    items.Add(new SelectListItem { Value = project.ProjectId.ToString(), Text = project.Name });
                }
            }

            return items;
        }

        public static bool InsertRequirementType(RequirementType rType)
        {
            bool success = false;
            rType.TenantId = Auth.GetTenantId();

            using (var db = new DataClassesDataContext())
            {
                db.RequirementTypes.InsertOnSubmit(rType);
                db.SubmitChanges();
                success = true;
            }

            return success;
        }

        public static bool UpdateRequirementType(RequirementType rType)
        {
            bool success = false;

            using (var db = new DataClassesDataContext())
            {
                var target = db.RequirementTypes.Where(t => t.TenantId == Auth.GetTenantId() && t.TypeId == rType.TypeId).FirstOrDefault();
                if (target != null)
                {
                    target.Name = rType.Name;
                    db.SubmitChanges();
                    success = true;
                }
            }

            return success;
        }
    }
}