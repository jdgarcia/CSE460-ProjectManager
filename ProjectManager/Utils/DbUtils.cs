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
    }
}