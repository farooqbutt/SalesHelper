using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Services;

namespace SalesHelper.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerService _customerService;
        private readonly AddressService _addressService;
        private readonly EventService _eventService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public CustomersController(
            EventService eventService,
            SignInManager<ApplicationUser> signInManager,
            CustomerService customerService,
            AddressService addressService
            )
        {
            _customerService = customerService;
            _addressService = addressService;
            _signInManager = signInManager;
            _eventService = eventService;
        }

        public Customer SetAccountNumAndCreatorOfCustomer(Customer customer)
        {
            customer.AccountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
            customer.CreatedByUserId = _signInManager.UserManager.GetUserAsync(User).Result.Id;
            return customer;
        }

        // check if customer name is kind of first name or full name


        [HttpPost]
        public IActionResult AddCustomerFromSearchBar(string name, string phone)
        {
            string customerFirstName = _customerService.DetermineNameType(name) == ICustomerService.NameType.First ? name : name.Split(" ")[0];
            string customerLastName = _customerService.DetermineNameType(name) == ICustomerService.NameType.Full ? name.Split(" ")[1] : "";
            var customer = new Customer
            {
                FirstName = customerFirstName,
                LastName = customerLastName,
                CellPhone = phone,
            };
            SetAccountNumAndCreatorOfCustomer(customer);
            var address = _addressService.Create(new Address());
            address.AddressType = "home";
            _addressService.Update(address);
            customer.AddressId = address.AddressId;
            _customerService.Create(customer);
            return Json(new { data = customer });
        }

        [HttpGet]
        public IActionResult AddCustomers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomers(AddCustomerInterface customerDetails)
        {
            SetAccountNumAndCreatorOfCustomer(customerDetails.Customer);
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
            SetAccountNumAndCreatorOfCustomer(customerDetails.Customer);
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
