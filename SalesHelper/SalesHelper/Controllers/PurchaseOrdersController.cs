using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Services;

namespace SalesHelper.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private readonly PurchaseOrderService _purchaseOrderService;
        public PurchaseOrdersController(PurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }
        [HttpGet]
        public IActionResult AllDraftOrdersView()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CabinetInformation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllDraftOrders()
        {
            return Json(new { data = _purchaseOrderService.ReadAll().Where(a => a.Status == "Draft").OrderByDescending(a => a.OrderDate) });
        }

        [HttpGet]
        public IActionResult CreateCabinetDraftOrder()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCabinetDraftOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.Status = "Draft";
            _purchaseOrderService.Create(purchaseOrder);
            return RedirectToAction("AllDraftOrdersView");
        }
    }
}
