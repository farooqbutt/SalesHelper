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
        public SaleStatus SalesOrder { get; set; }
        public string SalesNumber { get; set; }
        public Address HomeAddress { get; set; }
        public string Customer { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string CreatedBy { get; set; }
        public Address ShippingAddress { get; set; }
        public string Name { get; set; }

        public string PostalCode { get; set; }
        public decimal? SubTotal { get; set; }
    }

    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public string Label3 { get; set; }
        public string Label4 { get; set; }
    }


}
