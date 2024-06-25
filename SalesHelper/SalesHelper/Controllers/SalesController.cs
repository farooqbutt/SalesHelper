using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Services;

namespace SalesHelper.Controllers
{
    public class SalesController : Controller
    {
        private readonly SalesService _salesService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SalesController(SalesService salesService, SignInManager<ApplicationUser> signInManager)
        {
            _salesService = salesService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult SalesList()
        {
            return View((List<Sale>?)_salesService.ReadAll(_signInManager.UserManager.GetUserAsync(User).Result.AccountNumber));
        }

        [HttpGet]
        public IActionResult CreateSales(int? quoteId, string? quoteType)
        {
            if (quoteId != null && quoteType != null)
            {
                return View(_salesService.CreateSaleForView(quoteId.Value, quoteType));
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult CreateSales(Sale sale)
        {
            _salesService.Create(sale);
            return View();
        }
    }
}
