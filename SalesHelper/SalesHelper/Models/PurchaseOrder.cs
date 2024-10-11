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
        public string? OrderNumberByVendor { get; set; } = string.Empty;
        public string? OrderType { get; set; } = string.Empty;
        public string? OrderTypeForSalesHelper { get; set; }
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
        public DateTime? ShippingDate { get; set; }

        // Cabinet Information
        public string? CabinetLine { get; set; }
        public string? BoxConstruction { get; set; }
        public string? BoxMaterial { get; set; }
        public string? DoorDesignStyle { get; set; }
        public string? DoorMaterial { get; set; }
        public string? DrawerHead { get; set; }
        public string? CabinetColor { get; set; }
        public string? CabinetGlaze { get; set; }
        public string? IslandColor { get; set; }
        public string? IslandGlaze { get; set; }
        public string? FrameOption { get; set; }
        public string? HingeTypeInset { get; set; }
        public string? HingeTypeOverlay { get; set; }
        public string? Comments { get; set; }
        public string? AdditionalFieldsJson { get; set; }

        // Countertop Information
        public string? Material { get; set; }
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? RoomType { get; set; }
        public string? EdgeProfile { get; set; }
        public bool MiteredEdge { get; set; } = false;
        public bool FarmhouseSink { get; set; } = false;
        public bool Cooktop { get; set; } = false;
        public bool FullBacksplash { get; set; } = false;
        public bool MiteredEdgeOnIsland { get; set; } = false;
        public bool IslandPrepSink { get; set; } = false;
        public bool Rangetop { get; set; } = false;
        public bool FourIncBacksplash { get; set; } = false;
        public bool WaterfallOnIsland { get; set; } = false;

        public string? EstimateSquareFeet { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; }
    }
}
