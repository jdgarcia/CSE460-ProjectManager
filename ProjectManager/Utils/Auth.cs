using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManager.Models;

namespace ProjectManager.Utils
{
    public class Auth
    {
        public static bool Login(LoginContext loginInfo)
        {
            bool success = false;

            using (var db = new DataClassesDataContext())
            {
                User matchedUser = (from u in db.Users
                                    where u.Username == loginInfo.Username && u.Password == loginInfo.Password
                                    select u).FirstOrDefault();

                if (matchedUser != null)
                {
                    Login(matchedUser);
                    success = true;
                }
            }

            return success;
        }

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

        public static bool IsLoggedIn()
        {
            return (GetCurrentUser() != null);
        }

        public static CurrentUserContext GetCurrentUser()
        {
            return (CurrentUserContext)HttpContext.Current.Session["CurrentUser"];
        }
    }
}