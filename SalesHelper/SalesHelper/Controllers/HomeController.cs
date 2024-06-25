using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Services;
using System.Diagnostics;

namespace SalesHelper.Controllers
{
    public class HomeController : Controller
    {
        private readonly CustomerService _customerService;
        private readonly CabinetQuotationService _cabinetQuotationService;
        private readonly CountertopQuotationService _countertopQuotationService;
        private readonly SalesService _salesService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(
            CustomerService customerService,
            CabinetQuotationService cabinetQuotationService,
            SalesService salesService,
            CountertopQuotationService countertopQuotationService,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _customerService = customerService;
            _cabinetQuotationService = cabinetQuotationService;
            _countertopQuotationService = countertopQuotationService;
            _salesService = salesService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                ViewData["totalCustomers"] = _customerService.ReadAll().Where(a => a.AccountNumber ==
                    _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber).Count();
                ViewData["totalQuotations"] = _cabinetQuotationService.ReadAll().Where(a => a.CreatedByUserId ==
                    _signInManager.UserManager.GetUserAsync(User).Result.Id).Count() +
                _countertopQuotationService.ReadAll().Where(a => a.CreatedByUserId ==
                    _signInManager.UserManager.GetUserAsync(User).Result.Id).Count();
                ViewData["totalSales"] = _salesService.ReadAll(
                    _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber).Count();
                return View();
            }
            else
            {
                return View();
            }
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