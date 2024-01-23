using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models.CabinetCatalog
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ManufactureCode { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string ManufactureSectionName { get; set; } = string.Empty;

        [ForeignKey("CabinetryVendorIdFK")]
        public int CabinetryVendorId { get; set; }
        public Vendor CabinetryVendorIdFK { get; set; } = new Vendor();
    }
}
