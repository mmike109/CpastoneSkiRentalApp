using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CpastroneSkiRental.Models;
using Microsoft.AspNetCore.Authorization;

namespace CpastroneSkiRental.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Home page
        /// </summary>
        /// <returns>Home page</returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
           
        }
        /// <summary>
        /// Contact us page
        /// </summary>
        /// <returns>Contact us page</returns>
        [AllowAnonymous]
        public IActionResult Privacy()
        {

            try
            {
                return View();
            }
            catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Universal error page
        /// </summary>
        /// <returns>error page</returns>
        [AllowAnonymous]
        public IActionResult ErrorOccured()
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
