using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Data;
using SalesHelper.Models;
using System.Net.Mime;

namespace SalesHelper.Controllers
{
    public class VendorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        public VendorsController(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _signInManager = signInManager;
            _env = env;
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
                var vendorsList = _context.VendorReference.ToArray();
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
                var myVendorsList = _context.Vendor.Where(a => a.AccountNumberId == accountNumber).ToList();
                foreach (var item in myVendorsList)
                {
                    item.BusinessTypeIdFK = _context.BusinessTypes.Find(item.BusinessTypeId)!;
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

        [HttpGet]
        public IActionResult EditMyVendor(int id)
        {
            var vendor = _context.Vendor.Find(id)!;
            var editVendor = new EditMyVendorInterface();
            if (vendor.VendorReferenceId != null)
            {
                var vendorReference = _context.VendorReference.Find(vendor.VendorReferenceId)!;
                editVendor = new EditMyVendorInterface
                {
                    VendorId = vendor.VendorId,
                    CompanyName = vendorReference.CompanyName,
                    Email = vendorReference.Email,
                    MainPhone = vendorReference.MainPhone,
                    Fax = vendorReference.Fax,
                    PrimaryContact = "",
                    PrimaryContactPhone = "",
                    Website = vendorReference.Website,
                    Description = vendorReference.Description,
                    BusinessTypeId = vendorReference.BusinessTypeId,
                    IsPrivate = vendor.IsPrivate,
                    AccountNumberId = (int)vendor.AccountNumberId!,
                    AddressDetails = new List<Address>()
                    {
                        new Address
                        {
                            Address1 = vendorReference.Address1!,
                            Address2 = vendorReference.Address2!,
                            City = vendorReference.City!,
                            State = vendorReference.State!,
                            PostalCode = vendorReference.PostalCode!,
                            Country = vendorReference.Country!,
                            AddressType = "business"
                        },
                        new Address
                        {
                            Address1 = vendorReference.Address1!,
                            Address2 = vendorReference.Address2!,
                            City = vendorReference.City!,
                            State = vendorReference.State!,
                            PostalCode = vendorReference.PostalCode!,
                            Country = vendorReference.Country!,
                            AddressType = "billing"
                        },
                        new Address
                        {
                            Address1 = vendorReference.Address1!,
                            Address2 = vendorReference.Address2!,
                            City = vendorReference.City!,
                            State = vendorReference.State!,
                            PostalCode = vendorReference.PostalCode!,
                            Country = vendorReference.Country!,
                            AddressType = "shipping"
                        }
                    },
                    VendorReferenceId = vendor.VendorReferenceId,
                };
                return View(editVendor);
            }
            else
            {
                editVendor = new EditMyVendorInterface
                {
                    VendorId = vendor.VendorId,
                    CompanyName = vendor.CompanyName,
                    Email = vendor.Email,
                    MainPhone = vendor.MainPhone,
                    Fax = vendor.Fax,
                    PrimaryContact = vendor.PrimaryContact,
                    PrimaryContactPhone = vendor.PrimaryContactPhone,
                    Website = vendor.Website,
                    Description = vendor.Description,
                    BusinessTypeId = vendor.BusinessTypeId,
                    IsPrivate = vendor.IsPrivate,
                    AccountNumberId = (int)vendor.AccountNumberId!,
                    AddressDetails = new List<Address>
                    {
                        _context.Address.Find(vendor.BusinessAddressId)!,
                        _context.Address.Find(vendor.BillingAddressId)!,
                        _context.Address.Find(vendor.ShippingAddressId)!
                    },
                    VendorReferenceId = vendor.VendorReferenceId,
                    VendorDocumentsList = _context.VendorDocuments.Where(a => a.VendorId == vendor.VendorId).ToList(),
                };
                return View(editVendor);
            }
        }
        [HttpPost]
        public IActionResult EditMyVendor(EditMyVendorInterface vendor)
        {
            foreach (var item in vendor.AddressDetails)
            {
                if (item.AddressId == 0)
                {
                    _context.Address.Add(item);
                    _context.SaveChanges();
                    UpdateVendorAddress(vendor.VendorId, item.AddressId, item.AddressType);
                }
                else
                {
                    _context.Address.Update(item);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("EditMyVendor", new { id = vendor.VendorId });
        }

        [HttpPost]
        public async Task<IActionResult> UploadVendorDocument(EditMyVendorInterface vendor)
        {
            try
            {
                if (vendor.VendorFile != null && vendor.VendorFile.Length > 0)
                {
                    // Check the file size
                    if (vendor.VendorFile.Length > 1024 * 1024) // 1MB in bytes
                    {
                        // return error
                        var err = new { message = "error", result = "The file size should not exceed 1MB." };
                        return Json(err);
                    }
                    else
                    {
                        // Specify the folder where you want to save the uploaded files
                        string uploadFolder = Path.Combine(_env.WebRootPath, "VendorDocuments");

                        // Ensure the folder exists, or create it if it doesn't
                        Directory.CreateDirectory(uploadFolder);

                        // Get the file extension
                        string fileExtension = Path.GetExtension(vendor.VendorFile.FileName);

                        // Generate a unique file name to avoid overwriting existing files
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + vendor.VendorDocuments.DocumentName + fileExtension;

                        // Combine the folder path with the unique file name
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        var document = new VendorDocuments
                        {
                            DocumentName = uniqueFileName,
                            Description = vendor.VendorDocuments.Description,
                            FilePath = filePath,
                            UploadDate = DateTime.Now,
                            VendorId = vendor.VendorId,
                        };
                        await _context.VendorDocuments.AddAsync(document);
                        await _context.SaveChangesAsync();

                        // Save the file to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await vendor.VendorFile.CopyToAsync(fileStream);
                        }
                        return RedirectToAction("EditMyVendor", new { id = vendor.VendorId });
                    }
                }
                // return error
                var data = new { message = "error", result = "File not found!" };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        public IActionResult ShowVendorFile(string filePath)
        {
            // Check if the file exists
            if (System.IO.File.Exists(filePath))
            {
                // Set the Content-Disposition header to specify how the browser should handle the file
                var contentDisposition = new ContentDisposition
                {
                    FileName = Path.GetFileName(filePath),
                    Inline = true // Set to true to display the file in the browser, or false to force download.
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                // Serve the file with the appropriate content type
                var stream = System.IO.File.OpenRead(filePath);
                return File(stream, GetContentType(Path.GetExtension(filePath))); // Adjust the content type based on the file type.
            }
            else
            {
                var data = new { message = "error", result = "An Error Occurred" };
                return Json(data);
            }
        }

        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".pdf":
                    return "application/pdf";
                // Add more cases for other file types as needed.
                default:
                    return "application/octet-stream"; // Default to binary data if the file type is unknown.
            }
        }

        public void UpdateVendorAddress(int vendorId, int addressId, string addressType)
        {
            var vendor = _context.Vendor.Find(vendorId)!;
            if (addressType == "business")
            {
                vendor.BusinessAddressId = addressId;
            }
            if (addressType == "billing")
            {
                vendor.BillingAddressId = addressId;
            }
            if (addressType == "shipping")
            {
                vendor.ShippingAddressId = addressId;
            }
            _context.Vendor.Update(vendor);
            _context.SaveChanges();
        }

        [HttpPost]
        public IActionResult AddVendor(Vendor vendor)
        {
            try
            {
                if (vendor.VendorId == 0)
                {
                    if (vendor.IsPrivate == false)
                    {
                        var vendorReference = new VendorReference
                        {
                            CompanyName = vendor.CompanyName,
                            MainPhone = vendor.MainPhone,
                            Fax = vendor.Fax,
                            Email = vendor.Email,
                            Website = vendor.Website,
                            Description = vendor.Description,
                            BusinessTypeId = vendor.BusinessTypeId
                        };
                        _context.VendorReference.Add(vendorReference);
                        _context.SaveChanges();

                        //creating new vendor with vendor reference
                        vendor.AccountNumberId = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                        vendor.VendorReferenceId = vendorReference.VendorReferenceId;
                        _context.Vendor.Add(vendor);
                        _context.SaveChanges();
                    }
                    else
                    {
                        //creating new vendor without vendor reference
                        vendor.AccountNumberId = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                        _context.Vendor.Add(vendor);
                        _context.SaveChanges();
                    }
                    return RedirectToAction(nameof(ViewMyVendors));
                }
                else
                {
                    _context.Vendor.Update(vendor);
                    _context.SaveChanges();
                    return RedirectToAction("EditMyVendor", new { id = vendor.VendorId });
                }
            }
            catch (Exception ex)
            {
                var data = new { message = "error", result = ex.InnerException?.Message };
                return Json(data);
            }
        }

        public JsonResult AddToVendor(int vendorId)
        {
            try
            {
                var accountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
                var referenceVendor = _context.VendorReference.Find(vendorId)!;
                var vendorToAdd = new Vendor
                {
                    CompanyName = referenceVendor.CompanyName,
                    Email = referenceVendor.Email,
                    MainPhone = referenceVendor.MainPhone,
                    Fax = referenceVendor.Fax!,
                    Website = referenceVendor.Website,
                    Description = referenceVendor.Description,
                    BusinessTypeId = referenceVendor.BusinessTypeId,
                    IsPrivate = false,
                    AccountNumberId = accountNumber,
                    VendorReferenceId = referenceVendor.VendorReferenceId
                };
                // checking if vendor already exist
                var ifVendorExist = _context.Vendor.Where(a => a.VendorReferenceId == referenceVendor.VendorReferenceId)
                    .FirstOrDefault();
                if (ifVendorExist != null)
                {
                    return Json(new { message = "error", result = "Vendor already exists" });
                }
                _context.Vendor.Add(vendorToAdd);
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
                var vendor = _context.Vendor.Where(a => a.VendorId == vendorId).FirstOrDefault()!;
                _context.Vendor.Remove(vendor);
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
