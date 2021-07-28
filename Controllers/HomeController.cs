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
        public IActionResult Register(Models.UserAccountViewModel userAccount)
        {
            if (ModelState.IsValid)
            {
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
        //public IActionResult Login(Models.UserAccountViewModel userAccount)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return View("ClassList", userAccount);
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

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
