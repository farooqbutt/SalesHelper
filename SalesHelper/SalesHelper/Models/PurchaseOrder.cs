using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public string OrderName { get; set; } = string.Empty;

        [ForeignKey("SalesOrderFK")]
        public int? SalesOrderId { get; set; }
        public Sale SalesOrderFK { get; set; } = default!;

        [ForeignKey("CustomerFK")]
        public int CustomerId { get; set; }
        public Customer CustomerFK { get; set; } = default!;

        [ForeignKey("VendorFK")]
        public int VendorId { get; set; }
        public Vendor VendorFK { get; set; } = default!;

        public int? PurchaseOrderNumber { get; set; }
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? PurchaseContactName { get; set; } = string.Empty;
        public string? PurchaseContactPhone { get; set; } = string.Empty;
        public string? PurchaseContactEmail { get; set; } = string.Empty;
        public string? MethodOfShipping { get; set; } = string.Empty;
        public string? ShippingAddress1 { get; set; } = string.Empty;
        public string? ShippingAddress2 { get; set; } = string.Empty;
        public string? ShippingCountry { get; set; } = string.Empty;
        public string? ShippingState { get; set; } = string.Empty;
        public string? ShippingCity { get; set; } = string.Empty;
        public string? ShippingPostalCode { get; set; } = string.Empty;
        public string? ShippingContactName { get; set; } = string.Empty;
        public string? ShippingContactPhone { get; set; } = string.Empty;

        // Cabinet Information
        public string? Construction { get; set; } = string.Empty;
        public string? BoxMaterial { get; set; } = string.Empty;
        public string? WoodSpecie { get; set; }
        public string? DoorStyle { get; set; } = string.Empty;
        public string? CabinetFinish { get; set; } = string.Empty;
        public string? Comments { get; set; } = string.Empty;
    }
}
