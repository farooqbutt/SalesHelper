using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Services;

namespace SalesHelper.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private readonly VendorService _vendorService;
        private readonly PurchaseOrderService _purchaseOrderService;
        public PurchaseOrdersController(PurchaseOrderService purchaseOrderService, VendorService vendorService)
        {
            _vendorService = vendorService;
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public IActionResult AllOrders()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            return Json(new { data = _purchaseOrderService.ReadAll().OrderByDescending(a => a.ModifiedDate) });
        }
        [HttpGet]
        public IActionResult AllDraftOrdersView()
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
            purchaseOrder.OrderTypeForSalesHelper = "Cabinetry";
            purchaseOrder.Status = "Draft";
            _purchaseOrderService.Create(purchaseOrder);
            return Json(new { data = purchaseOrder });
        }

        [HttpPost]
        public IActionResult CreateCabinetOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.OrderTypeForSalesHelper = "Cabinetry";
            purchaseOrder.Status = "Ordered";
            _purchaseOrderService.CreatePONumber(purchaseOrder.CustomerFK.AccountNumber);
            _purchaseOrderService.Create(purchaseOrder);
            return Json(new { data = purchaseOrder });
        }

        [HttpGet]
        public IActionResult CabinetInformation(int vendorId, int purchaseOrderId)
        {
            TempData["Vendor"] = _vendorService.Read(vendorId).CompanyName;
            TempData["PurchaseOrderId"] = purchaseOrderId;
            return View();
        }

        [HttpPost]
        public IActionResult CabinetInformationUpdate(PurchaseOrder purchaseCabinetInformation)
        {
            var previousOrderData = _purchaseOrderService.Read(purchaseCabinetInformation.Id);
            previousOrderData.CabinetLine = purchaseCabinetInformation.CabinetLine;
            previousOrderData.BoxConstruction = purchaseCabinetInformation.BoxConstruction;
            previousOrderData.BoxMaterial = purchaseCabinetInformation.BoxMaterial;
            previousOrderData.DoorDesignStyle = purchaseCabinetInformation.DoorDesignStyle;
            previousOrderData.DoorMaterial = purchaseCabinetInformation.DoorMaterial;
            previousOrderData.DrawerHead = purchaseCabinetInformation.DrawerHead;
            previousOrderData.CabinetColor = purchaseCabinetInformation.CabinetColor;
            previousOrderData.CabinetGlaze = purchaseCabinetInformation.CabinetGlaze;
            previousOrderData.IslandColor = purchaseCabinetInformation.IslandColor;
            previousOrderData.IslandGlaze = purchaseCabinetInformation.IslandGlaze;
            previousOrderData.FrameOption = purchaseCabinetInformation.FrameOption;
            previousOrderData.HingeTypeInset = purchaseCabinetInformation.HingeTypeInset;
            previousOrderData.HingeTypeOverlay = purchaseCabinetInformation.HingeTypeOverlay;
            previousOrderData.Comments = purchaseCabinetInformation.Comments;
            previousOrderData.AdditionalFieldsJson = purchaseCabinetInformation.AdditionalFieldsJson;
            _purchaseOrderService.Update(previousOrderData);

            return Json(new { data = previousOrderData });
        }

        [HttpPost]
        public IActionResult CabinetInformationUpdateWithPO(PurchaseOrder purchaseCabinetInformation)
        {
            var previousOrderData = _purchaseOrderService.Read(purchaseCabinetInformation.Id);
            previousOrderData.CabinetLine = purchaseCabinetInformation.CabinetLine;
            previousOrderData.BoxConstruction = purchaseCabinetInformation.BoxConstruction;
            previousOrderData.BoxMaterial = purchaseCabinetInformation.BoxMaterial;
            previousOrderData.DoorDesignStyle = purchaseCabinetInformation.DoorDesignStyle;
            previousOrderData.DoorMaterial = purchaseCabinetInformation.DoorMaterial;
            previousOrderData.DrawerHead = purchaseCabinetInformation.DrawerHead;
            previousOrderData.CabinetColor = purchaseCabinetInformation.CabinetColor;
            previousOrderData.CabinetGlaze = purchaseCabinetInformation.CabinetGlaze;
            previousOrderData.IslandColor = purchaseCabinetInformation.IslandColor;
            previousOrderData.IslandGlaze = purchaseCabinetInformation.IslandGlaze;
            previousOrderData.FrameOption = purchaseCabinetInformation.FrameOption;
            previousOrderData.HingeTypeInset = purchaseCabinetInformation.HingeTypeInset;
            previousOrderData.HingeTypeOverlay = purchaseCabinetInformation.HingeTypeOverlay;
            previousOrderData.Comments = purchaseCabinetInformation.Comments;
            previousOrderData.AdditionalFieldsJson = purchaseCabinetInformation.AdditionalFieldsJson;
            previousOrderData.PurchaseOrderNumber = _purchaseOrderService.CreatePONumber(previousOrderData.CustomerFK.AccountNumber);
            _purchaseOrderService.Update(previousOrderData);

            return Json(new { data = previousOrderData });
        }

        [HttpGet]
        public IActionResult POSummary(int id)
        {
            return View(_purchaseOrderService.Read(id));
        }

        [HttpPost]
        public void SaveOrderNumberByVendor(int id, string orderNumber)
        {
            var po = _purchaseOrderService.Read(id);
            if (po != null)
            {
                po.OrderNumberByVendor = orderNumber;
                _purchaseOrderService.Update(po);
            }
        }

        [HttpPost]
        public void SaveShippingDateByVendor(int id, DateTime shippingDate)
        {
            var po = _purchaseOrderService.Read(id);
            if (po != null)
            {
                po.ShippingDate = shippingDate;
                _purchaseOrderService.Update(po);
            }

        }

        [HttpPost]
        public void UpdatePOStatus(int id, string status)
        {
            var po = _purchaseOrderService.Read(id);
            if (po != null)
            {
                po.Status = status;
                _purchaseOrderService.Update(po);
            }
        }
    }
}
