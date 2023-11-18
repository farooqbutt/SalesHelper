using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Repository;

namespace SalesHelper.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerRepo _customerService;
        private readonly AddressRepo _addressService;
        public CustomersController(
            CustomerRepo customerService,
            AddressRepo addressService
            )
        {
            _customerService = customerService;
            _addressService = addressService;
        }

        [HttpGet]
        public IActionResult AddCustomers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomers(AddCustomerInterface customerDetails)
        {
            _addressService.Create(customerDetails.Address);
            customerDetails.Customer.AddressId = customerDetails.Address.AddressId;
            _customerService.Create(customerDetails.Customer);
            return RedirectToAction("CustomerList");
        }

        [HttpGet]
        public IActionResult CustomerListView()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CustomerList()
        {
            var data = new { data = _customerService.ReadAll() };
            return Json(data);
        }

        [HttpGet]
        public IActionResult EditCustomer(int id)
        {
            var customer = new AddCustomerInterface
            {
                Customer = _customerService.Read(id),
                Address = _addressService.Read(_customerService.Read(id).AddressId)
            };
            return View(customer);
        }
        [HttpPost]
        public IActionResult EditCustomer(AddCustomerInterface customerDetails)
        {
            _customerService.Update(customerDetails.Customer);
            _addressService.Update(customerDetails.Address);
            return RedirectToAction("CustomerDetailedView", new { id = customerDetails.Customer.Id });
        }

        [HttpGet]
        public IActionResult CustomerDetailedView(int id)
        {
            ViewBag.CustomerId = id;
            return View();
        }

        [HttpGet]
        public JsonResult CustomerDetailedView_Details(int id)
        {
            var data = new { data = _customerService.Read(id) };
            data.data.AddressIdFK = _addressService.Read(data.data.AddressId);
            return Json(data);
        }
    }
}
