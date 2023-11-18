using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using System.Diagnostics;

namespace SalesHelper.Controllers
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

        public IActionResult AnalyticsView()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult AccountSettings()
        {
            return View();
        }

        public IActionResult AccountView()
        {
            return View();
        }

        public IActionResult BusinessTypes()
        {
            return View();
        }

        public IActionResult Vendors()
        {
            return View();
        }

        public IActionResult VendorContact()
        {
            return View();
        }

        public IActionResult MyVendors()
        {
            return View();
        }

        public IActionResult MyVendorContact()
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