using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebSiteCoreProject1.Models;
using Microsoft.AspNetCore.Http; // used for sessions

namespace WebSiteCoreProject1.Controllers
{
    public class HomeController : Controller
    {
        const string SessionName = "_Name";
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        //todo: should I return an account created successfully page?
        [HttpPost]
        //public IActionResult Register(Models.UserAccountViewModel userAccount)
        public IActionResult Register(Models.UserModelLogin userFormSubmission) 
        {
            if (ModelState.IsValid)
            {
                var database = new minicstructorContext();
                database.User.Add(userFormSubmission);
                database.SaveChanges(); 
                return View("Index"); // just return to home page.
            }
            else
            {
                return View(); // return the current view with field validation.
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //todo: create the view ClassList and a model
        [HttpPost]
        public IActionResult Login(Models.UserModelLogin userFormSubmission)
        {            
            if (ModelState.IsValid)
            {
                var database = new minicstructorContext();
                foreach (var user in database.User)
                {
                    if (user.UserEmail == userFormSubmission.UserEmail)
                    {
                        if (user.UserPassword == userFormSubmission.UserPassword)
                        {
                            HttpContext.Session.SetString(SessionName, user.UserEmail);
                            return View();
                        }
                    }
                }
                return View();
            }
            return View();
        }

        public IActionResult ClassList()
        {
            var database = new minicstructorContext();
            List<ClassModel> classList = new List<ClassModel> { };
            foreach (var c in database.Class)
            {
                var cmodel = new ClassModel();
                cmodel.ClassDescription = c.ClassDescription;
                cmodel.ClassId = c.ClassId;
                cmodel.ClassName = c.ClassName;
                cmodel.ClassPrice = c.ClassPrice;

                classList.Add(cmodel);
            }

            return View("classList", classList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
