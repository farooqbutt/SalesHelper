using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class SalesCountertopItem
    {
        [Key]
        public int Id { get; set; }
        public string Material { get; set; } = string.Empty;
        public string? Brand { get; set; } 
        public string? Color { get; set; } 
        public int? VendorId { get; set; }
        public string? RoomType { get; set; } 
        public string? EdgeProfile { get; set; } 
        public decimal EstimatedSquareFeet { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public int? CountertopQuotationId { get; set; } // can be null if user is going to add new line item

        [ForeignKey("SaleFk")]
        public int SalesId { get; set; }
        public Sale SaleFk { get; set; } = default!;
    }
}
