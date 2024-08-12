using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Services;

namespace SalesHelper.Controllers
{
    public class SalesController : Controller
    {
        private readonly SalesService _salesService;
        private readonly CabinetQuotationService _cabinetQuotationService;
        private readonly CountertopQuotationService _countertopQuotationService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SalesController(
            SalesService salesService, 
            SignInManager<ApplicationUser> signInManager, 
            CabinetQuotationService cabinetQuotationService,
            CountertopQuotationService countertopQuotationService)
        {
            _salesService = salesService;
            _signInManager = signInManager;
            _cabinetQuotationService = cabinetQuotationService;
            _countertopQuotationService = countertopQuotationService;
        }

        [HttpGet]
        public List<CabinetDetail> GetCabinetItems(int quoteId)
        {
            return _cabinetQuotationService.GetCabinetItems(quoteId);
        }

        [HttpGet]
        public List<SalesCountertopItem> GetCountertopItems(int quoteId)
        {
            return _countertopQuotationService.GetCountertopItems(quoteId);
        }

        [HttpPost]
        public void UpdateStatus(int id, string status)
        {
            _salesService.UpdateStatus(id, status);
        }

        [HttpGet]
        public IActionResult SalesList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult SalesListData()
        {
            return Json(new { data = _salesService.ReadAll(_signInManager.UserManager.GetUserAsync(User).Result.AccountNumber) });
        }

        [HttpGet]
        public IActionResult CreateSales()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSales(CreateSaleInterface sale)
        {
            _salesService.Create(sale);
            return View();
        }

        [HttpPost]
        public IActionResult SalesPreview(CreateSaleInterface sale)
        {
            return _salesService.SalePreviewGenerator(sale);
        }

        [HttpGet]
        public JsonResult GetSalesByCustomerId(int customerId)
        {
            return Json(new
            {
                data = _salesService
                .ReadAll(_signInManager.UserManager.GetUserAsync(User).Result.AccountNumber)
                .Where(a => a.CustomerId == customerId)
                .Select(a => a.Id).ToList()
            });
        }
    }
}
