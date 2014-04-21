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
    }
}