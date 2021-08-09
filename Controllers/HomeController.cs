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
                            // TODO: ADD Claim and cookieAuth here

                            return Redirect("~/"); // Successful logon redirects to home page
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

            EnrollInClassModel enrollModel = EnrollInClassHelper();

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
                

                // Get the user from db by looking up the logged in userId stored in Session variable.
                var user_db_result = database.User
                    .Find(int.Parse(HttpContext.Session.GetString(SessionUserId)));

                var class_db_result = database.Class
                    .Find(enrollClassForm.ClassId);

                // Check if the user has already enrolled in this class
                var user_enrolled_db_result = database.UserClass
                    .Find(enrollClassForm.ClassId, int.Parse(HttpContext.Session.GetString(SessionUserId)));

                if (user_enrolled_db_result != null)
                {
                    ModelState.AddModelError("", "Already enrolled in this class!");
                    EnrollInClassModel enrollModel = EnrollInClassHelper();
                    return View("enrollinclass", enrollModel);
                }

                // this works to add to db
                var userClassTable = new UserClass 
                { 
                    ClassId = class_db_result.ClassId,
                    UserId = user_db_result.UserId,
                };
                database.UserClass.Add(userClassTable);
                database.SaveChanges();
                return View("studentclasses"); // Go to list of enrolled classes page.
            }
            else
            {
                return View(); // return the current view with field validation.
            }
        }

        private static EnrollInClassModel EnrollInClassHelper()
        {
            EnrollInClassModel enrollModel = new EnrollInClassModel();

            var database = new minicstructorContext();
            // This works for setting ClassId in dropdown!
            foreach (var c in database.Class)
            {
                enrollModel.ClassNameSelItemList.Add(
                    new SelectListItem { Text = c.ClassName, Value = c.ClassId.ToString() });
                enrollModel.ClassId = c.ClassId;
            }

            return enrollModel;
        }

        //public IActionResult StudentClasses()
        //{
        //    List<ClassModel> classList = GetDbUserClassData(int.Parse(HttpContext.Session.GetString(SessionUserId)));
        //    return View("studentclasses", classList);
        //}

        //private List<ClassModel> GetDbUserClassData(int userId)
        //{
        //    //TODO: FINISH THIS SECTION
        //    //var database = new minicstructorContext();

        //    // first query userClass for all claisId for userId
        //    //SELECT TOP(1000) [ClassId]
        //    //,[UserId]
        //    //FROM[mini - cstructor].[dbo].[UserClass]
        //    //where UserId = 2

        //    var userClass_db_result = database.UserClass
        //        .Select(uc => new UserClass()
        //        {
        //            ClassId = uc.ClassId,
        //            UserId = uc.UserId,
        //        }).Where(uc => uc.UserId == userId).ToList();

        //    List<ClassModel> classList = new List<ClassModel> { };

        //    foreach (var res in userClass_db_result)
        //    {
        //        var class_db_result = database.Class
        //            .Select(c => new ClassModel()
        //            {
        //                ClassId = c.ClassId,
        //                ClassDescription = c.ClassDescription,
        //                ClassName = c.ClassName,
        //                ClassPrice = c.ClassPrice,
        //            }).Where(c => c.ClassId == res.ClassId);
        //        classList.Add(class_db_result);
        //    }




        //    return classList;
        //}

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
