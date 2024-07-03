using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SalesHelper.Models
{
    public enum SaleStatus
    {
        Completed,
        Pending,
        Confirmed,
        [EnumMember(Value = "In Processing")]
        In_Processing,
        Ordered,
        Cancelled,
        Refunded,
        Backordered,
        Shipped,
        [EnumMember(Value = "Ready For Pickup")]
        Ready_For_Pickup,
        [EnumMember(Value = "Ready For Delivery")]
        Ready_For_Delivery,
        [EnumMember(Value = "Ready For 1st Delivery")]
        Ready_For_1st_Delivery,
        [EnumMember(Value = "Ready For 2nd Delivery")]
        Ready_For_2nd_Delivery
    }

    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CreatedByFK")]
        public string CreatedById { get; set; } = string.Empty;
        public ApplicationUser CreatedByFK { get; set; } = default!;

        [ForeignKey("CustomerFK")]
        public int CustomerId { get; set; }
        public Customer CustomerFK { get; set; } = default!;

        [ForeignKey("CabinetQuotationFK")]
        public int? CabinetQuotationId { get; set; }
        public CabinetQuotation CabinetQuotationFK { get; set; } = default!;

        [ForeignKey("CountertopQuotationFK")]
        public int? CountertopQuotationId { get; set; }
        public CountertopQuotation CountertopQuotationFK { get; set; } = default!;
        public DateTime SalesDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public SaleStatus Status { get; set; }
        public decimal? Balance { get; set; }
        public decimal? SubTotal { get; set; }
    }
}