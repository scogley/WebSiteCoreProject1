using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebSiteCoreProject1.Models;

namespace WebSiteCoreProject1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
                bool userEmailMatch = false;
                bool userPasswordMatch = false;
                foreach (var user in database.User)
                {
                    if (user.UserEmail == userFormSubmission.UserEmail)
                    {
                        userEmailMatch = true;
                        if (user.UserPassword == userFormSubmission.UserPassword)
                        {
                            userPasswordMatch = true;
                            break;
                        }
                    }
                }
                if (userEmailMatch && userPasswordMatch)
                {
                    // TODO: DO SOMETHING LIKE SET A SESSION VARIABLE?
                    // Create the LoggedIn session variable
                    // see this example for how to use session state in aspnetcore https://www.c-sharpcorner.com/article/how-to-use-session-in-asp-net-core/#:~:text=How%20To%20Use%20Sessions%20In%20ASP.NET%20Core%201,double%20click%20%E2%80%9DStartup.cs%E2%80%9D%20to%20configure%20the%20services.%20
                    // then check the variable in layout and use something like @Html.Action("IncrementCount")
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
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
