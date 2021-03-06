using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebSiteCoreProject1.Models;
using Microsoft.AspNetCore.Http; // For sessions
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
// used for authentication
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // For authorization

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
            _database = database; // Dependency Injection of the dbcontext using constructor injection.
        }

        public IActionResult Index()
        {
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
                // Check if user already exists
                var userCheck = _database.User
                    .FirstOrDefault(u => u.UserEmail == userFormSubmission.UserEmail.ToLower());
                if (userCheck != null)
                {
                    ModelState.AddModelError("", $"The Username {userFormSubmission.UserEmail} already exists! Please use a different user name.");
                    return View();
                }

                PasswordHasher hashpass = new PasswordHasher(userFormSubmission.UserPassword);

                // Create a new User instance with the submitted email/password.
                var user = new User()
                {
                    UserEmail = userFormSubmission.UserEmail.ToLower(),
                    UserPassword = hashpass.HashedPassword,
                    UserSalt = hashpass.Salt
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
            if (!ModelState.IsValid) return View();
            
            foreach (var user_db in _database.User)
            {
                if (user_db.UserEmail.ToLower() != userLoginFormData.UserEmail.ToLower())
                {
                    // No match for Email found skip to next iteration.
                    continue;
                }
                
                PasswordHasher hashpassForm = new PasswordHasher(
                    userLoginFormData.UserPassword,
                    user_db.UserPassword,
                    user_db.UserSalt);
                if(hashpassForm.IsHashMatch)
                //if (user_db.UserPassword == hashpassForm.HashedPassword)
                {   
                    LogonUser(user_db);
                    // Successful logon redirects to home page.
                    return Redirect("~/");
                }
            }
            // No match for user submitted user/pass was found in db.
            return View();
        }

        private void LogonUser(User user_db)
        {
            HttpContext.Session.SetString(SessionName, user_db.UserEmail);
            HttpContext.Session.SetString(SessionUserId, user_db.UserId.ToString());
            // Add Claim and cookieAuth here

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user_db.UserEmail),
                new Claim(ClaimTypes.Role, "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false,
                IssuedUtc = DateTimeOffset.UtcNow,
            };

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties).Wait();
        }

        public IActionResult LogOff()
        {
            HttpContext.Session.Remove(SessionName);
            HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/Home/Login");
        }

        [Authorize]
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

        [Authorize]
        [HttpGet]
        public IActionResult EnrollInClass()
        {
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
            // See this page for useful info implementing dropdown https://stackoverflow.com/questions/34624034/select-tag-helper-in-asp-net-core-mvc
            if (ModelState.IsValid)
            {
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
