using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SalesHelper.Models
{

    public class Sale
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

        [Key]
        public int Id { get; set; }
        
        [ForeignKey("CreatedByFK")] 
        public string CreatedById { get; set; } = string.Empty; 
        public ApplicationUser CreatedByFK { get; set; } = default!;

        [ForeignKey("CustomerFK")]
        public int CustomerId { get; set; } 
        public Customer CustomerFK { get; set; } = default!;
        public DateTime SalesDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalSalesOrderAmount { get; set; }
        public decimal AdvancePaid { get; set; }
    }
}
