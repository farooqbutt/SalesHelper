using SalesHelper.Models.CabinetCatalog;

namespace SalesHelper.Models.InterfaceModels
{
    public class CabinetQuotationItems
    {
        public CabinetQuotation CabinetQuotation { get; set; } = new CabinetQuotation();
        public List<string> DoorStylesNames { get; set; } = new List<string>();
        public List<CatalogItem> CatalogItems { get; set; } = new List<CatalogItem>();
        public List<QuotationItem> CabinetQuotationItem { get; set; } = new List<QuotationItem>();
    }
}
