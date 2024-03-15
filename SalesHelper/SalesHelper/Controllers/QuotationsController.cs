using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;
using SalesHelper.Services;
using SalesHelper.Services.EmailService;
using System.Net.Mime;
using SalesHelper.Services.QuestPDF;
using SalesHelper.Services.UserServices;

namespace SalesHelper.Controllers
{
    public class QuotationsController : Controller
    {
        private readonly CabinetQuotationService _cabinetQuotationService;
        private readonly CountertopQuotationService _countertopQuotationService;
        private readonly CustomerService _customerService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AddressService _addressService;
        private readonly EmailService _emailService;
        private readonly VendorService _vendorService;
        private readonly GeneratePDFService _generatePDFService;
        private readonly ProfileCheckService _profileCheckService;

        public QuotationsController(
            CabinetQuotationService cabinetQuotationRepo,
            CountertopQuotationService countertopQuotationRepo,
            CustomerService customerRepo,
            ApplicationDbContext context,
            IWebHostEnvironment env,
            SignInManager<ApplicationUser> signInManager,
            AddressService addressService,
            EmailService emailService,
            VendorService vendorService,
            GeneratePDFService generatePDFService,
            ProfileCheckService profileCheckService)
        {
            _cabinetQuotationService = cabinetQuotationRepo;
            _countertopQuotationService = countertopQuotationRepo;
            _customerService = customerRepo;
            _context = context;
            _env = env;
            _signInManager = signInManager;
            _addressService = addressService;
            _emailService = emailService;
            _vendorService = vendorService;
            _generatePDFService = generatePDFService;
            _profileCheckService = profileCheckService;
        }

        #region COMMON METHODS

        public CabinetQuotation TimeAndUserUpdate(CabinetQuotation cabinetQuotation)
        {
            cabinetQuotation.CreatedByUserId = _signInManager.UserManager.GetUserAsync(User).Result.Id;
            cabinetQuotation.ModifiedDateTime = DateTime.Now;
            return cabinetQuotation;
        }

        private string GetViewNameFromUrl(string url)
        {
            // Parse the URL
            Uri uri = new Uri(url);
            string path = uri.AbsolutePath;

            // Split the path by slashes
            string[] segments = path.Split('/');

            // Initialize the view name
            string viewName = string.Empty;

            // Check if the last segment is a number (ID)
            if (!string.IsNullOrEmpty(segments[segments.Length - 1]) && !int.TryParse(segments[segments.Length - 1], out _))
            {
                // If the last segment is not a number, use it as the view name
                viewName = segments[segments.Length - 1];
            }
            else
            {
                // If the last segment is a number, use the second-to-last segment as the view name
                viewName = segments[segments.Length - 2];
            }

            return viewName;
        }

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

        [HttpGet]
        public async Task<IActionResult> QuoteEmailsView(int quoteId, string folderName)
        {
            if (folderName == "Inbox")
            {
                var emails = await _emailService.GetQuoteInboxEmails(quoteId);
                return View(emails);
            }
            else if (folderName == "Sent")
            {
                var emails = await _emailService.GetQuoteSentEmails(quoteId);
                return View(emails);
            }
            else
            {
                return View(new QuoteEmailsInterface());
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmailMessageDetails(string messageId, string folderName)
        {
            if (folderName == "Inbox")
            {
                var message = await _emailService.GetInboxEmailMessageDetails(messageId);
                return Json(message);
            }
            else if (folderName == "Sent")
            {
                var message = await _emailService.GetSentEmailMessageDetails(messageId);
                return Json(message);
            }
            else
            {
                return Json(null);
            }
        }

        [HttpPost]
        public IActionResult SendSimpleEmail(string to, string subject, string body, IFormFile attachment, int quoteId, string quoteType)
        {
            try
            {
                var task = _emailService.SendSimpleEmail(to, subject, body, attachment,
                    new QuotationEmails
                    {
                        QuotationId = quoteId,
                        QuoteType = quoteType,
                        UserId = _signInManager.UserManager.GetUserAsync(User).Result.Id
                    });
                if (task.IsCompletedSuccessfully)
                {
                    return Json(new { message = "success", result = "Email Sent Successfully!" });
                }
                else
                {
                    return Json(new { message = "error", result = "Email Sending Failed!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "error", result = ex.Message });
            }
        }

        #endregion COMMON METHODS

        #region CABINET QUOTATIONS

        [HttpGet]
        public IActionResult EditCabinetQuotation(int id)
        {
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            return View(_cabinetQuotationService.CabinetQuoteInterface(cabinetQuotation));
        }

        [HttpPost]
        public IActionResult EditCabinetQuotation(CabinetQuoteInterface cabinetQuotation)
        {
            // Serializing additional information fields to JSON string
            var additionalInfo = new
            {
                Refrigerator = cabinetQuotation.Refrigerator,
                StoveAndCooktop = cabinetQuotation.StoveAndCooktop,
                Dishwasher = cabinetQuotation.Dishwasher,
                Hood = cabinetQuotation.Hood,
                BuiltInOven = cabinetQuotation.BuiltInOven,
                BuiltInDrawerMicrowave = cabinetQuotation.BuiltInDrawerMicrowave,
                Sink = cabinetQuotation.Sink,
                Comments = cabinetQuotation.Comments
            };
            var jsonData = JsonConvert.SerializeObject(additionalInfo);

            var quoteToUpdate = _cabinetQuotationService.Read(cabinetQuotation.Id);
            // updating only fields that are changed in cabinet quotation
            quoteToUpdate.Name = cabinetQuotation.Name;
            quoteToUpdate.CustomerId = cabinetQuotation.CustomerId;
            quoteToUpdate.VendorId = cabinetQuotation.VendorId;
            quoteToUpdate.Construction = cabinetQuotation.Construction;
            quoteToUpdate.BoxMaterials = cabinetQuotation.BoxMaterials;
            quoteToUpdate.isOneColorDesign = cabinetQuotation.isOneColorDesign;
            // one color design fields
            quoteToUpdate.WoodSpeciesForOneColorDesign = cabinetQuotation.WoodSpeciesForOneColorDesign;
            quoteToUpdate.DoorStyleForOneColorDesign = cabinetQuotation.DoorStyleForOneColorDesign;
            quoteToUpdate.CabinetFinishForOneColorDesign = cabinetQuotation.CabinetFinishForOneColorDesign;
            // multiple color design fields
            quoteToUpdate.UpperWoodSpeciesForMultipleColorDesign = cabinetQuotation.UpperWoodSpeciesForMultipleColorDesign;
            quoteToUpdate.UpperDoorStyleForMultipleColorDesign = cabinetQuotation.UpperDoorStyleForMultipleColorDesign;
            quoteToUpdate.UpperFinishForMultipleColorDesign = cabinetQuotation.UpperFinishForMultipleColorDesign;
            quoteToUpdate.LowerWoodSpeciesForMultipleColorDesign = cabinetQuotation.LowerWoodSpeciesForMultipleColorDesign;
            quoteToUpdate.LowerDoorStyleForMultipleColorDesign = cabinetQuotation.LowerDoorStyleForMultipleColorDesign;
            quoteToUpdate.LowerFinishForMultipleColorDesign = cabinetQuotation.LowerFinishForMultipleColorDesign;
            quoteToUpdate.IslandWoodSpeciesForMultipleColorDesign = cabinetQuotation.IslandWoodSpeciesForMultipleColorDesign;
            quoteToUpdate.IslandDoorStyleForMultipleColorDesign = cabinetQuotation.IslandDoorStyleForMultipleColorDesign;
            quoteToUpdate.IslandFinishForMultipleColorDesign = cabinetQuotation.IslandFinishForMultipleColorDesign;
            quoteToUpdate.CommentsOnMultiColorDesign = cabinetQuotation.CommentsOnMultiColorDesign;
            // additional information fields
            quoteToUpdate.AdditionalInformation = jsonData;
            // updating price fields
            quoteToUpdate.CabinetPrice = cabinetQuotation.CabinetPrice;
            quoteToUpdate.DeliveryCharge = cabinetQuotation.DeliveryCharge;
            quoteToUpdate.InstallationFee = cabinetQuotation.InstallationFee;
            quoteToUpdate.Tax = cabinetQuotation.Tax;
            quoteToUpdate.VendorPrice = cabinetQuotation.VendorPrice;
            quoteToUpdate.CommentOnPrice = cabinetQuotation.CommentOnPrice;

            _cabinetQuotationService.Update(quoteToUpdate);
            return RedirectToAction("CabinetQuotationDetailedView", new { id = cabinetQuotation.Id });
        }

        [HttpGet]
        public IActionResult CreateCabinetQuotation(int? customerId)
        {
            TempData["CustomerId"] = customerId != null ? customerId : null;
            return View();
        }

        [HttpPost]
        public IActionResult CreateCabinetQuotation(CabinetQuoteInterface cabinetQuotation)
        {
            // Serializing additional information fields to JSON string
            var additionalInfo = new
            {
                Refrigerator = cabinetQuotation.Refrigerator,
                StoveAndCooktop = cabinetQuotation.StoveAndCooktop,
                Dishwasher = cabinetQuotation.Dishwasher,
                Hood = cabinetQuotation.Hood,
                BuiltInOven = cabinetQuotation.BuiltInOven,
                BuiltInDrawerMicrowave = cabinetQuotation.BuiltInDrawerMicrowave,
                Sink = cabinetQuotation.Sink,
                Comments = cabinetQuotation.Comments
            };
            var jsonData = JsonConvert.SerializeObject(additionalInfo);
            cabinetQuotation.AdditionalInformation = jsonData;

            _cabinetQuotationService.Create(cabinetQuotation);
            cabinetQuotation.VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!;
            return RedirectToAction("CabinetQuotationDetailedView", new { id = cabinetQuotation.Id });
        }

        [HttpGet]
        public IActionResult CabinetQuotationDetailedView(int id)
        {
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            var data = new QuotationFullViewInterface
            {
                CabinetQuotation = _cabinetQuotationService.CabinetQuoteInterface(cabinetQuotation),
            };
            return View(data);
        }

        [HttpPost]
        public IActionResult CabinetQuotationDetailedView(QuotationFullViewInterface quoteData)
        {
            _cabinetQuotationService.Update(quoteData.CabinetQuotation);
            return RedirectToAction("FullCabinetQuoteView", new { id = quoteData.CabinetQuotation.Id });
        }

        [HttpGet]
        public IActionResult FullCabinetQuoteView(int id)
        {
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            return View(_cabinetQuotationService.CabinetQuoteInterface(cabinetQuotation));
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
        public IActionResult AttachCabinetItems(List<QuotationItem> itemsList)
        {
            try
            {
                if (itemsList.Count > 0)
                {
                    // update modified date time of cabinet quotation
                    _cabinetQuotationService.Update(TimeAndUserUpdate(_cabinetQuotationService.Read(itemsList.First().QuotationId)));
                    /////////////////////////////////
                    _context.QuotationItems.AddRange(itemsList);
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
        public IActionResult UpdateCabinetItem(int itemid, int quantity, string description, string hinge, string finish)
        {
            try
            {
                var item = _context.QuotationItems.Find(itemid);
                item!.Quantity = quantity;
                item.Description = description;
                item.Hinge = hinge;
                item.Finish = finish;

                _context.QuotationItems.Update(item);
                _context.SaveChanges();

                // update modified date time of cabinet quotation
                _cabinetQuotationService.Update(TimeAndUserUpdate(_cabinetQuotationService.Read(item.QuotationId)));

                return Json(new { message = "success", result = "Item Updated Successfully!" });
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

        [HttpGet]
        public IActionResult CabinetQuotePrintPreview(int id)
        {
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            return View(_cabinetQuotationService.CabinetQuoteInterface(cabinetQuotation));
        }

        [HttpGet]
        public IActionResult RequestEstimateView(int id)
        {
            if (!_profileCheckService.IsProfileComplete(_signInManager.UserManager.GetUserAsync(User).Result.AccountNumber))
            {
                TempData["Message"] = "Please Complete Your Profile First!";
                return RedirectToAction("ProfileView", "AccountSettings");
            }
            var cabinetQuotation = _cabinetQuotationService.Read(id);
            return View(_cabinetQuotationService.CabinetQuoteInterface(cabinetQuotation));
        }

        [HttpPost]
        public IActionResult SendCabinetQuoteEstimateRequest(string to, string subject, string body, string attachmentName, int quoteId, IFormFile attachment)
        {
            try
            {
                var task = _emailService.SendEstimateRequestEmail(to, subject, body, attachmentName,
                           _generatePDFService.GenerateCabinetQuoteEstimateRequestPDF(quoteId, User), attachment, new QuotationEmails
                           {
                               QuotationId = quoteId,
                               QuoteType = "Cabinet Quote",
                               UserId = _signInManager.UserManager.GetUserAsync(User).Result.Id
                           });
                if (task.IsCompletedSuccessfully)
                {
                    return Json(new { message = "success", result = "Estimate Request Email Sent Successfully!" });
                }
                else
                {
                    return Json(new { message = "error", result = "Estimate Request Email Sending Failed!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "error", result = ex.Message });
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
        public IActionResult EditCountertopQuotation(int id)
        {
            var quotation = _countertopQuotationService.Read(id);
            quotation.CustomerIdFk = _customerService.Read(quotation.CustomerId);
            var data = new CountertopQuotationCreateInterface
            {
                CountertopQuotation = quotation,
                CountertopMaterials = _context.CountertopMaterials.Where(a => a.CountertopQuotationId == id).Include(a => a.VendorIdFk).ToList()
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

        [HttpGet]
        public IActionResult CountertopQuoteDetailedView(int id)
        {
            var quotation = _countertopQuotationService.Read(id);
            quotation.CustomerIdFk = _customerService.Read(quotation.CustomerId);
            quotation.CustomerIdFk.AddressIdFK = _addressService.Read(quotation.CustomerIdFk.AddressId);
            var quotationMaterials = _context.CountertopMaterials.Where(a => a.CountertopQuotationId == id).Include(a => a.VendorIdFk).ToList();
            var data = new CountertopQuotationCreateInterface
            {
                CountertopQuotation = quotation,
                CountertopMaterials = quotationMaterials
            };
            return View(data);
        }

        [HttpGet]
        public IActionResult CountertopPriceInquiry(int quoteId, int vendorId)
        {
            if (!_profileCheckService.IsProfileComplete(_signInManager.UserManager.GetUserAsync(User).Result.AccountNumber))
            {
                TempData["Message"] = "Please Complete Your Profile First!";
                return RedirectToAction("ProfileView", "AccountSettings");
            }

            var quotation = _countertopQuotationService.Read(quoteId);
            quotation.CustomerIdFk = _customerService.Read(quotation.CustomerId);
            quotation.CustomerIdFk.AddressIdFK = _addressService.Read(quotation.CustomerIdFk.AddressId);
            var quotationMaterials = _context.CountertopMaterials.Where(a => a.CountertopQuotationId == quoteId &&
                                                                             a.VendorId == vendorId).Include(a => a.VendorIdFk).ToList();
            var data = new CountertopQuotationCreateInterface
            {
                CountertopQuotation = quotation,
                CountertopMaterials = quotationMaterials
            };
            return View(data);
        }

        [HttpPost]
        public IActionResult SendCountertopQuoteEstimateRequest(string to, string subject, string body, string attachmentName, int quoteId, int vendorId, IFormFile attachment)
        {
            try
            {
                var task = _emailService.SendEstimateRequestEmail(to, subject, body, attachmentName,
                           _generatePDFService.GenerateCountertopQuoteEstimateRequestPDF(quoteId, vendorId, User), attachment, new QuotationEmails
                           {
                               QuotationId = quoteId,
                               QuoteType = "Countertop Quote",
                               UserId = _signInManager.UserManager.GetUserAsync(User).Result.Id,
                           });
                if (task.IsCompletedSuccessfully)
                {
                    return Json(new { message = "success", result = "Estimate Request Email Sent Successfully!" });
                }
                else
                {
                    return Json(new { message = "error", result = "Estimate Request Email Sending Failed!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "error", result = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetBrandsForMaterial(string material)
        {
            return Json(_countertopQuotationService.GetBrandsForMaterial(material));
        }

        [HttpGet]
        public IActionResult GetColorsForBrand(string brand, string color)
        {
            return Json(_countertopQuotationService.GetColorsForBrand(brand, color));
        }

        #endregion COUNTERTOP QUOTATIONS

        #region QUOTATION DOCUMENTS UPLOAD AND DELETE METHODS

        [HttpPost]
        public async Task<IActionResult> UploadQuotationDocuments(CabinetQuoteInterface quoteData)
        {
            // get name of view from where this method is being called
            string refererView = Request.Headers["Referer"].ToString();
            string viewName = GetViewNameFromUrl(refererView);
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
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                        // Combine the folder path with the unique file name
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        var document = new QuotationDocument
                        {
                            DocumentName = uniqueFileName,
                            Description = file.FileName,
                            FilePath = filePath,
                            UploadDate = DateTime.Now,
                            QuotationId = quoteData.Id,
                            QuotationIdFK = _cabinetQuotationService.Read(quoteData.Id)
                        };

                        await _context.QuotationDocuments.AddAsync(document);
                        await _context.SaveChangesAsync();

                        // Save the file to the server
                        using var fileStream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }

                    return RedirectToAction(viewName, new { id = quoteData.Id });
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

        public IActionResult ShowCabinetDcoument(string filePath)
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

        public IActionResult DeleteAttachedDocument(int documentId)
        {
            // get name of view from where this method is being called
            string refererView = Request.Headers["Referer"].ToString();
            string viewName = GetViewNameFromUrl(refererView);
            // get the id of quotation from document id before deleting it
            var quotationId = _context.QuotationDocuments.Find(documentId)!.QuotationId;
            // delete document from folder
            bool isDeleted = DeleteDocumentFromFolder(_context.QuotationDocuments.Find(documentId)!.FilePath);
            if (isDeleted)
            {
                // delete document record from database
                _context.QuotationDocuments.Remove(_context.QuotationDocuments.Find(documentId)!);
                _context.SaveChanges();
                return RedirectToAction(viewName, new { id = quotationId });
            }
            else
            {
                return Json(new { message = "error", result = "An Error Occurred" });
            }
        }

        public bool DeleteDocumentFromFolder(string filePath)
        {
            try
            {
                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Delete the file
                    System.IO.File.Delete(filePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static string GetContentType(string fileExtension)
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

        #endregion QUOTATION DOCUMENTS UPLOAD AND DELETE METHODS
    }
}
