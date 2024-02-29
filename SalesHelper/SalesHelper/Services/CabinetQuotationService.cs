using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services
{
    public class CabinetQuotationService : ICabinetQuotationService
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomerService _customerService;
        public CabinetQuotationService(ApplicationDbContext context, CustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }

        public CabinetQuoteInterface CabinetQuoteInterface(CabinetQuotation cabinetQuotation)
        {

            var cabinetQuoteInterface = new CabinetQuoteInterface
            {
                Id = cabinetQuotation.Id,
                Name = cabinetQuotation.Name,
                CustomerId = cabinetQuotation.CustomerId,
                CustomerIdFk = _customerService.Read(cabinetQuotation.CustomerId),
                VendorId = cabinetQuotation.VendorId,
                VendorIdFk = _context.Vendor.Find(cabinetQuotation.VendorId)!,
                Construction = cabinetQuotation.Construction,
                BoxMaterials = cabinetQuotation.BoxMaterials,
                isOneColorDesign = cabinetQuotation.isOneColorDesign,
                // one color
                WoodSpeciesForOneColorDesign = cabinetQuotation.WoodSpeciesForOneColorDesign,
                DoorStyleForOneColorDesign = cabinetQuotation.DoorStyleForOneColorDesign,
                CabinetFinishForOneColorDesign = cabinetQuotation.CabinetFinishForOneColorDesign,
                // multiple color
                UpperWoodSpeciesForMultipleColorDesign = cabinetQuotation.UpperWoodSpeciesForMultipleColorDesign,
                UpperDoorStyleForMultipleColorDesign = cabinetQuotation.UpperDoorStyleForMultipleColorDesign,
                UpperFinishForMultipleColorDesign = cabinetQuotation.UpperFinishForMultipleColorDesign,
                LowerWoodSpeciesForMultipleColorDesign = cabinetQuotation.LowerWoodSpeciesForMultipleColorDesign,
                LowerDoorStyleForMultipleColorDesign = cabinetQuotation.LowerDoorStyleForMultipleColorDesign,
                LowerFinishForMultipleColorDesign = cabinetQuotation.LowerFinishForMultipleColorDesign,
                IslandWoodSpeciesForMultipleColorDesign = cabinetQuotation.IslandWoodSpeciesForMultipleColorDesign,
                IslandFinishForMultipleColorDesign = cabinetQuotation.IslandFinishForMultipleColorDesign,
                IslandDoorStyleForMultipleColorDesign = cabinetQuotation.IslandDoorStyleForMultipleColorDesign,
                CommentsOnMultiColorDesign = cabinetQuotation.CommentsOnMultiColorDesign,
                // additional information
                AdditionalInformation = cabinetQuotation.AdditionalInformation,
                CreatedByUserId = cabinetQuotation.CreatedByUserId,
                CreatedDateTime = cabinetQuotation.CreatedDateTime,
                ModifiedDateTime = cabinetQuotation.ModifiedDateTime,
                CabinetPrice = cabinetQuotation.CabinetPrice,
                DeliveryCharge = cabinetQuotation.DeliveryCharge,
                InstallationFee = cabinetQuotation.InstallationFee,
                Tax = cabinetQuotation.Tax,
                VendorPrice = cabinetQuotation.VendorPrice,
                CommentOnPrice = cabinetQuotation.CommentOnPrice,
                Refrigerator = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Refrigerator"],
                StoveAndCooktop = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["StoveAndCooktop"],
                Dishwasher = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Dishwasher"],
                Hood = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Hood"],
                BuiltInOven = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["BuiltInOven"],
                BuiltInDrawerMicrowave = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["BuiltInDrawerMicrowave"],
                Sink = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Sink"],
                Comments = JsonConvert.DeserializeObject<dynamic>(cabinetQuotation.AdditionalInformation!)!["Comments"]
            };
            return cabinetQuoteInterface;
        }

        public void Create(CabinetQuotation cabinetQuotation)
        {
            try
            {
                _context.CabinetQuotations.Add(cabinetQuotation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.CabinetQuotations.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public CabinetQuotation Read(int id)
        {
            try
            {
                return _context.CabinetQuotations.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<CabinetQuotation> ReadAll()
        {
            try
            {
                return _context.CabinetQuotations.Include(a => a.VendorIdFk).Include(a => a.CustomerIdFk).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(CabinetQuotation cabinetQuotation)
        {
            try
            {
                _context.CabinetQuotations.Update(cabinetQuotation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
