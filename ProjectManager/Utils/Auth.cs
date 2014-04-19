using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManager.Models;

namespace ProjectManager.Utils
{
    public class Auth
    {
        public static void Login(User user)
        {
            CurrentUserContext currentUser = new CurrentUserContext();
            currentUser.Id = user.UserId;
            currentUser.Username = user.Username;

            using (var db = new DataClassesDataContext())
            {
                Admin admin = (from a in db.Admins
                               where a.TenantId == user.TenantId && a.Username == user.Username
                               select a).FirstOrDefault();
                currentUser.IsAdmin = (admin != null);
            }

            HttpContext.Current.Session["CurrentUser"] = currentUser;
        }

        public static void Logout()
        {
            HttpContext.Current.Session["CurrentUser"] = null;
        }

        public static CurrentUserContext GetCurrentUser()
        {
            return (CurrentUserContext)HttpContext.Current.Session["CurrentUser"];
        }
    }
}