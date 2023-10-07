using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Controllers
{
    public class AccountSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountSettingsController(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }
        public IActionResult ProfileView()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAccount()
        {
            try
            {
                var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                var account = _context.Account.Find(accountNumber);
                var data = new { message = "success", result = account };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult CreateAccount(Account account)
        {
            try
            {
                if (account.AccountNumber == 0)
                {
                    // New Account
                    _context.Account.Add(account);
                    _context.SaveChanges();
                }
                else
                {
                    // Update Account
                    _context.Account.Update(account);
                    _context.SaveChanges();
                }

                var data = new { message = "success", result = account };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpGet]
        public JsonResult GetAddress()
        {
            try
            {
                var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                var account = _context.Account.Find(accountNumber)!;
                var businessAddress = _context.Address.Find(account.BusinessAddressId);
                var billingAddress = _context.Address.Find(account.BillingAddressId);
                var shippingAddress = _context.Address.Find(account.ShippingAddressId);

                var data = new
                {
                    message = "success",
                    result = new
                    {
                        businessAddress,
                        billingAddress,
                        shippingAddress
                    }
                };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult CreateAddress(Address address)
        {
            try
            {
                // Create New Address
                if (address.AddressId == 0)
                {
                    _context.Address.Add(address);
                    _context.SaveChanges();
                    // Update Account with AddressId
                    var accountid = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                    var account = _context.Account.Find(accountid);
                    if (address.AddressType == "business")
                    {
                        account!.BusinessAddressId = address.AddressId;
                    }
                    else if (address.AddressType == "billing")
                    {
                        account!.BillingAddressId = address.AddressId;
                    }
                    else if (address.AddressType == "shipping")
                    {
                        account!.ShippingAddressId = address.AddressId;
                    }
                    // Update Account
                    _context.Account.Update(account!);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Address.Update(address);
                    _context.SaveChanges();
                }

                var data = new { message = "success", result = GetAddress() };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpGet]
        public JsonResult GetAccountBilling()
        {
            try
            {
                var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                var account = _context.Account.Find(accountNumber)!;
                var accountBilling = _context.AccountBilling.Where(a => a.BillingAddressId == account.BillingAddressId);
                var data = new { message = "success", result = accountBilling };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult CreateAccountBilling(AccountBilling accountBilling)
        {
            try
            {
                if (accountBilling.BillingId == 0)
                {   // New Account Billing
                    var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                    var account = _context.Account.Find(accountNumber)!;
                    accountBilling.AccountNumber = account.AccountNumber;
                    accountBilling.BillingAddressId = account.BillingAddressId;
                    _context.AccountBilling.Add(accountBilling);
                    _context.SaveChanges();
                }
                else
                {   // Update Account Billing
                    _context.AccountBilling.Update(accountBilling);
                    _context.SaveChanges();
                }

                var data = new { message = "success", result = accountBilling };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpGet]
        public JsonResult GetAccountShipping()
        {
            try
            {
                var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                var account = _context.Account.Find(accountNumber)!;
                var accountShipping = _context.AccountShipping.Where(a => a.ShippingAddressId ==
                account.ShippingAddressId).FirstOrDefault();
                var data = new { message = "success", result = accountShipping };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult CreateAccountShipping(AccountShipping accountShipping)
        {
            try
            {
                if (accountShipping.ShippingId == 0)
                {   // New Account Shipping
                    var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                    var account = _context.Account.Find(accountNumber)!;
                    accountShipping.AccountNumber = account.AccountNumber;
                    accountShipping.ShippingAddressId = account.ShippingAddressId;
                    _context.AccountShipping.Add(accountShipping);
                    _context.SaveChanges();
                }
                else
                {   // Update Account Shipping
                    _context.AccountShipping.Update(accountShipping);
                    _context.SaveChanges();
                }

                var data = new { message = "success", result = accountShipping };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }
    }
}