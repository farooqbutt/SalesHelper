using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Repository;
using System.Net.Mime;

namespace SalesHelper.Controllers
{
    public class QuotationsController : Controller
    {
        private readonly CabinetQuotationRepo _cabinetQuotationService;
        private readonly CountertopQuotationRepo _countertopQuotationService;
        private readonly CustomerRepo _customerService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AddressRepo _addressService;

        public QuotationsController(
            CabinetQuotationRepo cabinetQuotationRepo,
            CountertopQuotationRepo countertopQuotationRepo,
            CustomerRepo customerRepo,
            ApplicationDbContext context,
            IWebHostEnvironment env,
            SignInManager<ApplicationUser> signInManager,
            AddressRepo addressService)
        {
            _cabinetQuotationService = cabinetQuotationRepo;
            _countertopQuotationService = countertopQuotationRepo;
            _customerService = customerRepo;
            _context = context;
            _env = env;
            _signInManager = signInManager;
            _addressService = addressService;
        }

        #region COMMON METHODS START

        [HttpGet]
        public IActionResult QuotationsListView()
        {
            return View();
        }

        [HttpGet]
        public JsonResult QuotationsList()
        {
            // right now this is only for getting cabinet quotations
            //TODO: add other quotations logic here as well
            var cabinetQuotations = _cabinetQuotationService.ReadAll().Where(a => a.CreatedByUserId ==
                            _signInManager.UserManager.GetUserAsync(User).Result.Id).OrderByDescending(a => a.ModifiedDateTime).ToList();
            var countertopQuotations = _countertopQuotationService.ReadAll().Where(a => a.CreatedByUserId ==
                                       _signInManager.UserManager.GetUserAsync(User).Result.Id).OrderByDescending(a => a.ModifiedDateTime).ToList();
            var quotationsList = new List<QuotationsListVM>();

            foreach (var item in cabinetQuotations)
            {
                var quotation = new QuotationsListVM
                {
                    QuotationId = QuotationsListVM.QuoteTypesIntials.CBNT.ToString() + "-" + item.Id,
                    QuotationName = item.Name,
                    Customer = item.CustomerIdFk,
                    QuoteType = QuotationsListVM.QuoteTypes.Cabinet.ToString(),
                    Vendor = item.VendorIdFk,
                    CreatedDate = item.CreatedDateTime,
                    ModifiedDate = item.ModifiedDateTime
                };
                quotationsList.Add(quotation);
            }

            foreach (var item in countertopQuotations)
            {
                var quotation = new QuotationsListVM
                {
                    QuotationId = QuotationsListVM.QuoteTypesIntials.CNTP.ToString() + "-" + item.Id,
                    QuotationName = item.Name,
                    Customer = item.CustomerIdFk,
                    QuoteType = QuotationsListVM.QuoteTypes.Countertop.ToString(),
                    Vendor = null,
                    CreatedDate = item.CreatedDateTime,
                    ModifiedDate = item.ModifiedDateTime
                };
                quotationsList.Add(quotation);
            }

            var data = new { data = quotationsList.OrderByDescending(a => a.ModifiedDate) };

            return Json(data);
        }

        #endregion COMMON METHODS END

        #region CABINET QUOTATIONS

        public CabinetQuotation TimeAndUserUpdate(CabinetQuotation cabinetQuotation)
        {
            cabinetQuotation.CreatedByUserId = _signInManager.UserManager.GetUserAsync(User).Result.Id;
            cabinetQuotation.ModifiedDateTime = DateTime.Now;
            return cabinetQuotation;
        }

        [HttpGet]
        public IActionResult EditCabinetQuotation(int id)
        {
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            cabinetQuotation.CustomerIdFk = _customerService.Read(cabinetQuotation.CustomerId);
            cabinetQuotation.VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!;
            return View(cabinetQuotation);
        }

        [HttpPost]
        public IActionResult EditCabinetQuotation(CabinetQuotation cabinetQuotation)
        {
            var quoteToUpdate = _cabinetQuotationService.Read(cabinetQuotation.Id);
            // updating only fields that are changed in cabinet quotation
            quoteToUpdate.Name = cabinetQuotation.Name;
            quoteToUpdate.CustomerId = cabinetQuotation.CustomerId;
            quoteToUpdate.VendorId = cabinetQuotation.VendorId;
            quoteToUpdate.Construction = cabinetQuotation.Construction;
            quoteToUpdate.BoxMaterials = cabinetQuotation.BoxMaterials;
            quoteToUpdate.WoodSpeciesForOneColorDesign = cabinetQuotation.WoodSpeciesForOneColorDesign;
            quoteToUpdate.DoorStyleForOneColorDesign = cabinetQuotation.DoorStyleForOneColorDesign;
            quoteToUpdate.CabinetFinishForOneColorDesign = cabinetQuotation.CabinetFinishForOneColorDesign;
            quoteToUpdate.DoorStyleForMultipleColorDesign = cabinetQuotation.DoorStyleForMultipleColorDesign;
            quoteToUpdate.UpperWoodSpeciesForMultipleColorDesign = cabinetQuotation.UpperWoodSpeciesForMultipleColorDesign;
            quoteToUpdate.LowerWoodSpeciesForMultipleColorDesign = cabinetQuotation.LowerWoodSpeciesForMultipleColorDesign;
            quoteToUpdate.IslandWoodSpeciesForMultipleColorDesign = cabinetQuotation.IslandWoodSpeciesForMultipleColorDesign;
            quoteToUpdate.UpperFinishForMultipleColorDesign = cabinetQuotation.UpperFinishForMultipleColorDesign;
            quoteToUpdate.LowerFinishForMultipleColorDesign = cabinetQuotation.LowerFinishForMultipleColorDesign;
            quoteToUpdate.IslandFinishForMultipleColorDesign = cabinetQuotation.IslandFinishForMultipleColorDesign;
            quoteToUpdate.CommentsOnMultiColorDesign = cabinetQuotation.CommentsOnMultiColorDesign;

            _cabinetQuotationService.Update(TimeAndUserUpdate(quoteToUpdate));
            cabinetQuotation.VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!;
            return RedirectToAction("CabinetQuotationAdditionalInformation", new { id = cabinetQuotation.Id });
        }

        [HttpGet]
        public IActionResult CreateCabinetQuotation(int? customerId)
        {
            TempData["CustomerId"] = customerId != null ? customerId : null;
            return View();
        }

        [HttpPost]
        public IActionResult CreateCabinetQuotation(CabinetQuotation cabinetQuotation)
        {
            cabinetQuotation.CreatedDateTime = DateTime.Now;
            _cabinetQuotationService.Create(TimeAndUserUpdate(cabinetQuotation));
            cabinetQuotation.VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!;
            return RedirectToAction("CabinetQuotationAdditionalInformation", new { id = cabinetQuotation.Id });
        }

        [HttpGet]
        public IActionResult CabinetQuotationAdditionalInformation(int id)
        {
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            cabinetQuotation.CustomerIdFk = _customerService.Read(cabinetQuotation.CustomerId);
            cabinetQuotation.VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!;
            var data = new QuotationFullViewInterface
            {
                CabinetQuotation = cabinetQuotation
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CabinetQuotationAdditionalInformation(QuotationFullViewInterface quoteData)
        {
            _cabinetQuotationService.Update(TimeAndUserUpdate(quoteData.CabinetQuotation));
            // if document file is not null then upload it
            if (quoteData.DocumentFiles != null && quoteData.DocumentFiles.Count > 0)
            {
                var data = await UploadQuotationDocuments(quoteData);
                // get message value from data.value object
                dynamic obj = data.Value!;
                if (obj.message != "error")
                {
                    return RedirectToAction("FullCabinetQuoteView", new { id = quoteData.CabinetQuotation.Id });
                }
                else
                {
                    return Json(new { status = "error", message = obj.result });
                }
            }
            return RedirectToAction("FullCabinetQuoteView", new { id = quoteData.CabinetQuotation.Id });
        }

        [HttpGet]
        public IActionResult FullCabinetQuoteView(int id)
        {
            var fullCabinetQuotation = _cabinetQuotationService.Read(id);
            fullCabinetQuotation.CustomerIdFk = _customerService.Read(fullCabinetQuotation.CustomerId);
            fullCabinetQuotation.VendorIdFk = _context.Vendor.Find(fullCabinetQuotation.VendorId)!;
            return View(fullCabinetQuotation);
        }

        [HttpGet]
        public IActionResult AttachCabinetItems(int id)
        {
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            cabinetQuotation.CustomerIdFk = _customerService.Read(cabinetQuotation.CustomerId);
            cabinetQuotation.VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!;
            var doorStyles = _context.CabinetryDoorStyles.Where(
                                a => a.CatalogItemIdFK.CabinetryVendorIdFK.VendorId == cabinetQuotation.VendorId
                            ).Select(a => a.DoorStyleName).Distinct().ToList();
            var viewData = new CabinetQuotationItems
            {
                CabinetQuotation = cabinetQuotation,
                DoorStylesNames = doorStyles,
                CatalogItems = _context.CatalogItems.Where(a => a.CabinetryVendorIdFK.VendorId == cabinetQuotation.VendorId).ToList(),
                CabinetQuotationItem = new List<QuotationItem>()
            };
            return View(viewData);
        }

        [HttpPost]
        public IActionResult AttachCabinetItems(List<QuotationItem> ItemsList)
        {
            try
            {
                if (ItemsList.Count() > 0)
                {
                    // update modified date time of cabinet quotation
                    _cabinetQuotationService.Update(TimeAndUserUpdate(_cabinetQuotationService.Read(ItemsList.First().QuotationId)));
                    /////////////////////////////////
                    _context.QuotationItems.AddRange(ItemsList);
                    _context.SaveChanges();
                    return Json(new { message = "success", result = "Items Saved Successfully!" });
                }
                else
                {
                    return Json(new { message = "error", result = "Some error occure!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "error", result = ex.InnerException!.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteAttachedItem(int itemid)
        {
            try
            {
                // update modified date time of cabinet quotation
                _cabinetQuotationService.Update(TimeAndUserUpdate(_cabinetQuotationService.Read(_context.QuotationItems.Find(itemid)!.QuotationId)));
                /////////////////////////////////
                _context.QuotationItems.Remove(_context.QuotationItems.Find(itemid)!);
                _context.SaveChanges();
                return Json(new { message = "success", result = "Deleted Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { message = "success", result = ex.InnerException!.Message });
            }
        }

        [HttpGet]
        public JsonResult GetCabinetItem(int catalogItemId, string doorStyleName)
        {
            var cabinetItemData = _context.CabinetryDoorStyles.Where(a => a.CatalogItemId == catalogItemId
                                && a.DoorStyleName == doorStyleName).Include(a => a.CatalogItemIdFK).FirstOrDefault();
            return Json(cabinetItemData);
        }


        // Attaching Quotations Files and Documents

        [HttpPost]
        public async Task<JsonResult> UploadQuotationDocuments(QuotationFullViewInterface quoteData)
        {
            try
            {
                if (quoteData.DocumentFiles != null && quoteData.DocumentFiles.Count > 0)
                {
                    foreach (var file in quoteData.DocumentFiles)
                    {
                        // Check the file size
                        if (file.Length > 1024 * 1024 * 50) // 50MB in size
                        {
                            // return error
                            var err = new { message = "error", result = "The file size should not exceed 1MB." };
                            return Json(err);
                        }

                        // Specify the folder where you want to save the uploaded files
                        string uploadFolder = Path.Combine(_env.WebRootPath, "QuotationDocuments");

                        // Ensure the folder exists, or create it if it doesn't
                        Directory.CreateDirectory(uploadFolder);

                        // Get the file extension
                        string fileExtension = Path.GetExtension(file.FileName);

                        // Generate a unique file name to avoid overwriting existing files
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName + fileExtension;

                        // Combine the folder path with the unique file name
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        var document = new QuotationDocument
                        {
                            DocumentName = uniqueFileName,
                            Description = file.FileName,
                            FilePath = filePath,
                            UploadDate = DateTime.Now,
                            QuotationId = quoteData.CabinetQuotation.Id,
                            QuotationIdFK = quoteData.CabinetQuotation
                        };

                        await _context.QuotationDocuments.AddAsync(document);
                        await _context.SaveChangesAsync();

                        // Save the file to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    return Json(new { message = "success", result = "Files uploaded successfully!" });
                }

                // return error
                var data = new { message = "error", result = "No files found for upload!" };
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

        #endregion CABINET QUOTATIONS

        #region COUNTERTOP QUOTATIONS

        [HttpGet]
        public IActionResult CreateCountertopQuotationView(int? customerId)
        {
            TempData["CustomerId"] = customerId != null ? customerId : null;
            return View();
        }

        [HttpPost]
        public IActionResult CreateCountertopQuotation(CountertopQuotationCreateInterface quotation)
        {
            _countertopQuotationService.Create(quotation.CountertopQuotation);
            foreach (var item in quotation.CountertopMaterials)
            {
                item.CountertopQuotationId = quotation.CountertopQuotation.Id;
                _context.CountertopMaterials.Add(item);
            }
            _context.SaveChanges();
            return Json(new { message = "success", result = "Quotation Created Successfully!" });
        }

        [HttpGet]
        public IActionResult FullCountertopQuoteView(int id)
        {
            var quotation = _countertopQuotationService.Read(id);
            quotation.CustomerIdFk = _customerService.Read(quotation.CustomerId);
            quotation.CustomerIdFk.AddressIdFK = _addressService.Read(quotation.CustomerIdFk.AddressId);
            var quotationMaterials = _context.CountertopMaterials.Where(a => a.CountertopQuotationId == id).ToList();
            var data = new CountertopQuotationCreateInterface
            {
                CountertopQuotation = quotation,
                CountertopMaterials = quotationMaterials
            };
            return View(data);
        }

        [HttpGet]
        public IActionResult EditCountertopQuotation(int id)
        {
            var quotation = _countertopQuotationService.Read(id);
            quotation.CustomerIdFk = _customerService.Read(quotation.CustomerId);
            var data = new CountertopQuotationCreateInterface
            {
                CountertopQuotation = quotation,
                CountertopMaterials = _context.CountertopMaterials.Where(a => a.CountertopQuotationId == id).ToList()
            };
            return View(data);
        }

        [HttpPost]
        public IActionResult EditCountertopQuotation(CountertopQuotationCreateInterface quote)
        {
            // first delete all countertop materials related to this quotation
            _context.CountertopMaterials.RemoveRange(_context.CountertopMaterials.Where(a => a.CountertopQuotationId == quote.CountertopQuotation.Id));
            _context.SaveChanges();
            // update countertop quotation
            _countertopQuotationService.Update(quote.CountertopQuotation);
            // add countertop materials
            foreach (var item in quote.CountertopMaterials)
            {
                item.CountertopQuotationId = quote.CountertopQuotation.Id;
                _context.CountertopMaterials.Add(item);
            }
            _context.SaveChanges();
            return Json(new { message = "success", result = "Quotation Updated Successfully!" });
        }

        #endregion COUNTERTOP QUOTATIONS
    }
}
