using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ProjectManager.Models;

namespace ProjectManager.Utils
{
    public class Auth
    {
        public static bool Login(LoginContext loginInfo)
        {
            bool success = false;

            if (!string.IsNullOrEmpty(loginInfo.Username) && !string.IsNullOrEmpty(loginInfo.Password))
            {
                using (var db = new DataClassesDataContext())
                {
                    User matchedUser = (from u in db.Users
                                        where u.Username == loginInfo.Username && u.Password == GetPasswordHash(loginInfo.Password)
                                        select u).FirstOrDefault();

                    if (matchedUser != null)
                    {
                        Login(matchedUser);
                        success = true;
                    }
                }
            }

            return success;
        }

        public static void Login(User user)
        {
            CurrentUserContext currentUser = new CurrentUserContext();
            currentUser.TenantId = user.TenantId;
            currentUser.UserId = user.UserId;
            currentUser.Username = user.Username;
            currentUser.IsAdmin = (user.RoleId == 1);
            currentUser.IsManager = (user.RoleId == 4);

            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = (from t in db.Tenants
                                 where t.TenantId == currentUser.TenantId
                                 select t).FirstOrDefault();

                currentUser.BannerColor = tenant.BannerColor;
                currentUser.TextColor = tenant.TextColor;
                currentUser.TenantName = tenant.OrgName;
                currentUser.LogoPath = tenant.LogoPath;
            }

            HttpContext.Current.Session["CurrentUser"] = currentUser;

            HttpCookie userCookie = new HttpCookie("ProjectManagerUserSession");
            userCookie["username"] = user.Username;
            userCookie["password"] = user.Password;
        }

        public static void Logout()
        {
            HttpContext.Current.Session["CurrentUser"] = null;
        }

        public static bool IsLoggedIn()
        {
            return (GetCurrentUser() != null);
        }

        public static List<Models.User> GetManagersAndAdmins()
        {
            List<User> managers = new List<User>();
            int tID = GetCurrentUser().TenantId;
            using (var db = new DataClassesDataContext())
            {
                var possibleManagers = (from u in db.Users
                               where u.TenantId == tID &&
                               (u.RoleId == 1 || u.RoleId == 4) //1 is admin, 4 is manager
                               select u);
                foreach (var manager in possibleManagers)
                {
                    managers.Add(manager);
                }
            }
            return managers;
        }

        public static List<Models.User> GetWorkers() 
        {
            List<User> workers = new List<User>();
            int tID = GetCurrentUser().TenantId;
            using (var db= new DataClassesDataContext())
            {
                var result = (from u in db.Users
                               where u.TenantId == tID
                               && u.RoleId == 5
                               select u);

                foreach (var worker in result)
	            {
                    workers.Add(worker);
	            }
            }
            return workers;
        }

        public static List<Models.RequirementType> GetRequirementTypes()
        {
            List<RequirementType> types = new List<RequirementType>();
            int tID = GetCurrentUser().TenantId;
            using (var db = new DataClassesDataContext())
            {
                var result = (from t in db.RequirementTypes
                              where t.TenantId == tID
                              || t.TenantId == 1
                              select t);

                foreach (var type in result)
                {
                    types.Add(type);
                }
            }
            return types;
        }

        public static List<Models.Project> GetManagedProjects()
        {
            List<Project> projects = new List<Project>();
            int tID = GetCurrentUser().TenantId;
            int uID = GetCurrentUser().UserId;
            using (var db = new DataClassesDataContext())
            {
                var result = (from p in db.Projects
                              where p.TenantId == tID
                              && p.ManagerId == uID
                              select p);

                foreach (var proj in result)
                {
                    projects.Add(proj);
                }
            }
            return projects;
        }

        public static CurrentUserContext GetCurrentUser()
        {
            CurrentUserContext user = (CurrentUserContext)HttpContext.Current.Session["CurrentUser"];
            if (user == null || user.TenantId == null)
            {
                return user;
            }
            using (var db = new DataClassesDataContext())
            {
                Tenant tenant = (from t in db.Tenants
                                 where t.TenantId == user.TenantId
                                 select t).FirstOrDefault();
                
                user.LogoPath = tenant.LogoPath;
                user.TextColor = tenant.TextColor;
                user.BannerColor = tenant.BannerColor;
                user.TenantName = tenant.OrgName;
            }

            return user;
        }

        public static string GetPasswordHash(string password)
        {
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}