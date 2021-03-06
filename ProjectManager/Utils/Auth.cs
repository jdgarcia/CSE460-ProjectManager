﻿using System;
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
                currentUser.UsingCustomTypes = tenant.CustomTypes;
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
                user.UsingCustomTypes = tenant.CustomTypes;
            }

            return user;
        }

        public static int GetTenantId()
        {
            return ((CurrentUserContext)(HttpContext.Current.Session["CurrentUser"])).TenantId;
        }

        public static int GetUserId()
        {
            return ((CurrentUserContext)(HttpContext.Current.Session["CurrentUser"])).UserId;
        }

        public static bool IsAdmin()
        {
            return ((CurrentUserContext)(HttpContext.Current.Session["CurrentUser"])).IsAdmin;
        }

        public static bool IsManager()
        {
            return ((CurrentUserContext)(HttpContext.Current.Session["CurrentUser"])).IsManager;
        }

        public static bool IsUsingCustomTypes()
        {
            return ((CurrentUserContext)(HttpContext.Current.Session["CurrentUser"])).UsingCustomTypes;
        }

        public static string GetPasswordHash(string password)
        {
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}