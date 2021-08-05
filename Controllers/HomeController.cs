using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebSiteCoreProject1.Models;
using Microsoft.AspNetCore.Http; // used for sessions
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem

namespace WebSiteCoreProject1.Controllers
{
    public class HomeController : Controller
    {
        private const string SessionName = "_Name";
        private const string SessionUserId = "_UserId";
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Check whether a user is logged in and redirect to login page if not.
            if (HttpContext.Session.GetString(SessionName) == null )
            {
                Response.Redirect("/Home/Login");
            }
            ViewBag.Name = HttpContext.Session.GetString(SessionName); // store in ViewBag to access User from View.
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        //public IActionResult Register(Models.UserAccountViewModel userAccount)
        public IActionResult Register(Models.UserModelLogin userFormSubmission) 
        {
            if (ModelState.IsValid)
            {
                var database = new minicstructorContext();
                database.User.Add(userFormSubmission);
                database.SaveChanges(); 
                return View("Login"); // Go to login page.
            }
            else
            {
                return View(); // return the current view with field validation.
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            // Check whether a user is logged in and redirect to home page
            //if (HttpContext.Session.GetString(SessionName) != null)
            //{
            //    Response.Redirect("/");
            //}
            return View();
        }

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
                            HttpContext.Session.SetString(SessionUserId, user.UserId.ToString());
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
            List<ClassModel> classList = GetDbClassData();
            return View("classList", classList);
        }

        [HttpGet]
        public IActionResult EnrollInClass()
        {
            // Check whether a user is logged in and redirect to login page if not.
            if (HttpContext.Session.GetString(SessionName) == null)
            {
                Response.Redirect("/Home/Login");
            }
            ViewBag.Name = HttpContext.Session.GetString(SessionName); // store in ViewBag to access User from View.

            EnrollInClassModel enrollModel = new EnrollInClassModel();
            
            var database = new minicstructorContext();
            // This works for setting ClassId in dropdown!
            foreach (var c in database.Class)
            {
                enrollModel.ClassNameSelItemList.Add(
                    new SelectListItem { Text = c.ClassName, Value = c.ClassId.ToString() });
                enrollModel.ClassId = c.ClassId;
            }

            return View("enrollinclass", enrollModel);
        }

        [HttpPost]
        public IActionResult EnrollInClass(Models.EnrollInClassModel enrollClassForm)
        {
            // enrollClassForm has ClassId property set!
            // See this page for useful info implementing dropdown https://stackoverflow.com/questions/34624034/select-tag-helper-in-asp-net-core-mvc
            if (ModelState.IsValid)
            {
                var database = new minicstructorContext();

                // need to get the ClassId and the UserId to modify UserClass table
                // Get the current session user id and query db table for the
                // TODO: GET THIS WORKING!
                //var userId = database.UserClass
                //    .Select(u => new UserClass()
                //    //).Where(u => u.UserId.ToString() == HttpContext.Session.GetString(SessionUserId));
                //).Where(u => u.UserId.ToString() == "1002");


                // Get the user from db by looking up the logged in userId stored in Session variable.
                var user = database.User
                    .Find(int.Parse(HttpContext.Session.GetString(SessionUserId)));

                //var classId = database.Class
                //    .Find(enrollClassForm.)

                // this works to add to db
                var userClassTable = new UserClass 
                { 
                    ClassId = 1,
                    UserId = 2,
                };
                database.UserClass.Add(userClassTable);

                
                
                database.SaveChanges();
                return View("Login"); // Go to login page.
            }
            else
            {
                return View(); // return the current view with field validation.
            }
        }


        public IActionResult StudentClasses()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private static List<ClassModel> GetDbClassData()
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

            return classList;
        }
    }
}
