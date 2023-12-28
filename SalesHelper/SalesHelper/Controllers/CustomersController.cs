using Microsoft.AspNetCore.Identity;
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
        private readonly EventRepo _eventService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public CustomersController(
            EventRepo eventService,
            SignInManager<ApplicationUser> signInManager,
            CustomerRepo customerService,
            AddressRepo addressService
            )
        {
            _customerService = customerService;
            _addressService = addressService;
            _signInManager = signInManager;
            _eventService = eventService;
        }

        public Customer SetAccountNumAndCreatorOfEvent(Customer customer)
        {
            customer.AccountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
            customer.CreatedByUserId = _signInManager.UserManager.GetUserAsync(User).Result.Id;
            return customer;
        }

        [HttpGet]
        public IActionResult AddCustomers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomers(AddCustomerInterface customerDetails)
        {
            SetAccountNumAndCreatorOfEvent(customerDetails.Customer);
            _addressService.Create(customerDetails.Address);
            customerDetails.Customer.AddressId = customerDetails.Address.AddressId;
            _customerService.Create(customerDetails.Customer);
            return RedirectToAction("CustomerListView");
        }

        [HttpGet]
        public IActionResult CustomerListView()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CustomerList()
        {
            var customers = _customerService.ReadAll().Where(a => a.AccountNumber == _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber);
            List<CustomerListInterface> result = new List<CustomerListInterface>();
            foreach (var customer in customers)
            {
                result.Add(new CustomerListInterface
                {
                    CustomerObject = customer,
                    Events = _eventService.ReadAll().Where(a => a.CustomerId == customer.Id).
                    OrderBy(a => Convert.ToDateTime(a.Start)).Take(5).ToList()
                });
            }
            var data = new { data = result };
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
            SetAccountNumAndCreatorOfEvent(customerDetails.Customer);
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

        [HttpPost]
        public string DeleteCustomer(int id)
        {
            _customerService.Delete(id);
            return "Customer Deleted Successfully!";
        }
    }
}
