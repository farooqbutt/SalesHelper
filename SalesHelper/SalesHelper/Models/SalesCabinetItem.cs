using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class SalesCabinetItem
    {
        [Key]
        public int Id { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int VendorId { get; set; } // cabinetry vendor
        public string? Construction { get; set; } = string.Empty;
        public string? BoxMaterial { get; set; } = string.Empty;
        public string? WoodSpecie { get; set; }
        public string? DoorStyle { get; set; } = string.Empty;
        public string? CabinetFinish { get; set; } = string.Empty;
        public int? CabinetQuotationId { get; set; } // can be null if user is going to add new line item

        [ForeignKey("SaleFk")]
        public int SalesId { get; set; }
        public Sale SaleFk { get; set; } = default!;
    }
}