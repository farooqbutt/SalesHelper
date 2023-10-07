using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; }

        [Required]
        [MaxLength(50)]
        public string VendorName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Website { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("BusinessTypeIdFK")]
        public int BusinessTypeId { get; set; }
        public BusinessTypes BusinessTypeIdFK { get; set; } = default!;

        [Required]
        public bool IsPrivate { get; set; }


        [ForeignKey("BusinessAddressIdFK")]
        public int? BusinessAddressId { get; set; }
        public Address BusinessAddressIdFK { get; set; } = default!;

        [ForeignKey("ShippingAddressIdFK")]
        public int? ShippingAddressId { get; set; }
        public Address ShippingAddressIdFK { get; set; } = default!;

        [ForeignKey("BillingAddressIdFK")]
        public int? BillingAddressId { get; set; }
        public Address BillingAddressIdFK { get; set; } = default!;
    }
}
