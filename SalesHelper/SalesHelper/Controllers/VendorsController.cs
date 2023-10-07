using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Controllers
{
    public class VendorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public VendorsController(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult ViewVendors()
        {
            return View();
        }

        [HttpGet]
        public JsonResult VendorsList()
        {
            try
            {
                var vendorsList = _context.Vendor.Where(a => a.IsPrivate == false).ToArray();
                foreach (var item in vendorsList)
                {
                    item.BusinessTypeIdFK = _context.BusinessTypes.Find(item.BusinessTypeId)!;
                }
                var data = new { data = vendorsList };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        public IActionResult ViewMyVendors()
        {
            return View();
        }

        [HttpGet]
        public JsonResult MyVendorsList()
        {
            try
            {
                var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                var myVendorsList = _context.MyVendor.Where(a => a.AccountNumber == accountNumber).ToArray();
                foreach (var item in myVendorsList)
                {
                    item.VendorIdFK = _context.Vendor.Find(item.VendorId)!;
                    item.VendorIdFK.BusinessTypeIdFK = _context.BusinessTypes.Find(item.VendorIdFK.BusinessTypeId)!;
                }
                var data = new { data = myVendorsList };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpPost]
        public IActionResult AddVendor(Vendor vendor)
        {
            try
            {
                if (vendor.VendorId == 0)
                {
                    //creating new vendor
                    _context.Vendor.Add(vendor);
                    _context.SaveChanges();
                    //adding vendor to my vendors
                    var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                    var myVendor = new MyVendor
                    {
                        AccountNumber = accountNumber,
                        VendorId = vendor.VendorId
                    };
                    _context.MyVendor.Add(myVendor);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Vendor.Update(vendor);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(ViewMyVendors));
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        public JsonResult AddToMyVendor(int vendorId)
        {
            try
            {
                var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                var myVendor = new MyVendor
                {
                    AccountNumber = accountNumber,
                    VendorId = vendorId
                };
                _context.MyVendor.Add(myVendor);
                _context.SaveChanges();
                var data = new { message = "success", result = "Vendor added successfully" };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult DeleteMyVendor(int vendorId)
        {
            try
            {
                var vendor = _context.MyVendor.Where(a => a.VendorId == vendorId).FirstOrDefault()!;
                _context.MyVendor.Remove(vendor);
                _context.SaveChanges();
                var data = new { message = "success", result = "Vendor deleted successfully" };
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
