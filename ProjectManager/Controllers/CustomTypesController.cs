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

        // POST: /CustomType/Create

        [HttpPost]
        public ActionResult Create(string typeName)
        {
            if (!Auth.IsLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (!Auth.IsUsingCustomTypes())
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                return View();
            }

            RequirementType newType = new RequirementType();
            newType.Name = typeName;

            DbUtils.InsertRequirementType(newType);

            return RedirectToAction("Index");
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
