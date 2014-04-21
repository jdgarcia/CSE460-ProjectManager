using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Utils;

namespace ProjectManager.Controllers
{
    public class CustomTypesController : Controller
    {
        //
        // GET: /CustomTypes/

        public ActionResult Index()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!Auth.IsUsingCustomTypes())
            {
                return View("AccessDenied");
            }

            return View(DbUtils.GetCustomTypes());
        }

        //
        // GET: /CustomTypes/Create

        public ActionResult Create()
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!Auth.IsUsingCustomTypes())
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // GET: /CustomTypes/Edit/{id}

        public ActionResult Edit(int id)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!Auth.IsUsingCustomTypes())
            {
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
