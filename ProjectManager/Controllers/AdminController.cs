﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            CurrentUserContext currentUser = ProjectManager.Utils.Auth.GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            return View(currentUser);
        }

        //
        // GET: /Admin/Login

        public ActionResult Login()
        {
            return View();
        }
    }
}