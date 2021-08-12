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
        private minicstructorContext _database;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, minicstructorContext database)
        {
            _logger = logger;
            _database = database; // Dependency Injection of the dbcontext.
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

        public IActionResult Register(Models.UserModelRegister userFormSubmission) 
        {
            if (ModelState.IsValid)
            {
                // Create a new User instance with the submitted email/password.
                var user = new User()
                {
                    UserEmail = userFormSubmission.UserEmail,
                    UserPassword = userFormSubmission.UserPassword
                };
                _database.User.Add(user);
                _database.SaveChanges(); 
                return View("Login"); // Go to login page.
            }
            else
            {
                return View(); // return the current view with Model enforced field validation.
            }
        }
        [HttpGet]
        public IActionResult Login()
        {   
            return View();
        }

        [HttpPost]
        public IActionResult Login(Models.UserModelLogin userLoginFormData)
        {            
            if (ModelState.IsValid)
            {
                foreach (var user_db in _database.User)
                {
                    if (user_db.UserEmail == userLoginFormData.UserEmail)
                    {
                        if (user_db.UserPassword == userLoginFormData.UserPassword)
                        {
                            HttpContext.Session.SetString(SessionName, user_db.UserEmail);
                            HttpContext.Session.SetString(SessionUserId, user_db.UserId.ToString());
                            // TODO: ADD Claim and cookieAuth here

                            return Redirect("~/"); // Successful logon redirects to home page
                        }
                    }
                }
                return View();
            }
            return View();
        }

        public IActionResult StudentClasses()
        {
            List<ClassModel> classList = GetDbUserClassData(int.Parse(HttpContext.Session.GetString(SessionUserId)));
            return View("studentclasses", classList);
        }

        private List<ClassModel> GetDbUserClassData(int userId)
        {

            // First query userClass for all classId for userId
            var userClass_db_result = _database.UserClass
                .Select(uc => new UserClass()
                {
                    ClassId = uc.ClassId,
                    UserId = uc.UserId,
                }).Where(uc => uc.UserId == userId).ToList();

            List<ClassModel> classList = new List<ClassModel> { };

            // Then query Class for each classId in results.
            foreach (var res in userClass_db_result)
            {
                var class_db_result = _database.Class
                    .Select(c => new ClassModel()
                    {
                        ClassId = c.ClassId,
                        ClassDescription = c.ClassDescription,
                        ClassName = c.ClassName,
                        ClassPrice = c.ClassPrice,
                    })
                    .Where(c => c.ClassId == res.ClassId).ToList();
                classList.Add(class_db_result[0]); // Only one result add it to the list.
            }

            return classList;
        }

        public IActionResult ClassList()
        {
            List<ClassModel> classList = GetDbClassData(_database);
            return View("classList", classList);
        }

        private static List<ClassModel> GetDbClassData(minicstructorContext db)
        {
            
            List<ClassModel> classList = new List<ClassModel> { };
            foreach (var c in db.Class)
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

        [HttpGet]
        public IActionResult EnrollInClass()
        {
            // Check whether a user is logged in and redirect to login page if not.
            if (HttpContext.Session.GetString(SessionName) == null)
            {
                Response.Redirect("/Home/Login");
            }
            ViewBag.Name = HttpContext.Session.GetString(SessionName); // store in ViewBag to access User from View.

            EnrollInClassModel enrollModel = EnrollInClassHelper(_database);

            return View("enrollinclass", enrollModel);
        }

        private static EnrollInClassModel EnrollInClassHelper(minicstructorContext db)
        {
            EnrollInClassModel enrollModel = new EnrollInClassModel();

            // This works for setting ClassId in dropdown!
            foreach (var c in db.Class)
            {
                enrollModel.ClassNameSelItemList.Add(
                    new SelectListItem { Text = c.ClassName, Value = c.ClassId.ToString() });
                enrollModel.ClassId = c.ClassId;
            }

            return enrollModel;
        }

        [HttpPost]
        public IActionResult EnrollInClass(Models.EnrollInClassModel enrollClassForm)
        {
            // enrollClassForm has ClassId property set!
            // See this page for useful info implementing dropdown https://stackoverflow.com/questions/34624034/select-tag-helper-in-asp-net-core-mvc
            if (ModelState.IsValid)
            {
                
                // need to get the ClassId and the UserId to modify UserClass table
                // Get the current session user id and query db table for the
                

                // Get the user from db by looking up the logged in userId stored in Session variable.
                var user_db_result = _database.User
                    .Find(int.Parse(HttpContext.Session.GetString(SessionUserId)));

                var class_db_result = _database.Class
                    .Find(enrollClassForm.ClassId);

                // Check if the user has already enrolled in this class
                var user_enrolled_db_result = _database.UserClass
                    .Find(enrollClassForm.ClassId, int.Parse(HttpContext.Session.GetString(SessionUserId)));

                if (user_enrolled_db_result != null)
                {
                    ModelState.AddModelError("", $"You are already enrolled in the {class_db_result.ClassName} class!");
                    EnrollInClassModel enrollModel = EnrollInClassHelper(_database);
                    return View("enrollinclass", enrollModel);
                }

                // this works to add to db
                var userClassTable = new UserClass 
                { 
                    ClassId = class_db_result.ClassId,
                    UserId = user_db_result.UserId,
                };
                _database.UserClass.Add(userClassTable);
                _database.SaveChanges();
                return Redirect("~/Home/studentclasses"); // Go to list of enrolled classes page.
            }
            else
            {
                return View(); // return the current view with field validation.
            }
        }

        

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        
    }
}
