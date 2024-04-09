using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Data;
using SalesHelper.Models;
using hMailServer;
using SalesHelper.Services.EncryptionServices;

namespace SalesHelper.Controllers
{
    public class AccountSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly EncryptionDecryptionService _encdecService;

        public AccountSettingsController(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            EncryptionDecryptionService encdecService,
            IConfiguration config)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _encdecService = encdecService;
            _config = config;
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
        public JsonResult CreateAccount(Models.Account account, string password)
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
                    account.EncryptedSaleshelperEmailPassword = _encdecService.Encrypt(password);
                    _context.Account.Update(account);
                    var savedRecords = _context.SaveChanges();
                    if (savedRecords > 0)
                    {
                        HMailCreateAccount(account.SaleshelperMailingAddress!, password);
                    }
                    else
                    {
                        throw new Exception("No records updated");
                    }
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

        [HttpGet]
        public JsonResult CheckNameAvailability(string name)
        {
            try
            {
                var account = _context.Account.Where(a => a.SaleshelperMailingAddress == name).FirstOrDefault();
                if (account == null)
                {
                    var data = new { message = "success", result = "Name is available" };
                    return Json(data);
                }
                else
                {
                    var data = new { message = "error", result = "Name is not available" };
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpGet]
        public JsonResult VerifyUserPassword(string password)
        {
            bool IsPasswordVerified = _userManager.CheckPasswordAsync(_signInManager.UserManager.GetUserAsync(User).Result, password).Result;
            if (IsPasswordVerified == true)
            {
                return Json(new { data = true });
            }
            else
            {
                return Json(new { data = false });
            }
        }

        private Application HMailAuthenticate()
        {
            try
            {
                Application app = new();
                _ = app.Authenticate(
                    _config.GetValue<string>("HMailServerSettings:UserName"),
                    _config.GetValue<string>("HMailServerSettings:Password")
                    );
                return app;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while authenticating hMailServer: " + ex.Message);
            }
        }

        private void HMailCreateAccount(string companyName, string password)
        {
            try
            {
                Application hMailApp = HMailAuthenticate();
                Domain myDomain = hMailApp.Domains.ItemByName[_config.GetValue<string>("HMailServerSettings:Domain")];
                hMailServer.Account account = myDomain.Accounts.Add();
                account.Address = companyName;
                account.Password = password;
                account.Active = true;
                account.MaxSize = 500; // 500MB
                account.Save();
                _ = account.IMAPFolders.Add("Trash"); // creating Trash folder
                _ = account.IMAPFolders.Add("Sent");  // creating Sent folder
                account.IMAPFolders.ItemByName["Trash"].Subscribed = true; // Subscribing Trash folder
                account.IMAPFolders.ItemByName["Sent"].Subscribed = true;  // Subscribing Sent folder
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating hMail account: " + ex.Message);
            }
        }
    }
}