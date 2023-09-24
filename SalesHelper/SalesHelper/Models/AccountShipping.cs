using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class AccountShipping
    {
        [Key]
        public int ShippingId { get; set; }

        [MaxLength(50)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ShippingMethod { get; set; } = string.Empty;

        [ForeignKey("AccountNumberFK")]
        public int AccountNumber { get; set; }
        public virtual Account AccountNumberFK { get; set; } = default!;

        [ForeignKey("ShippingAddressFK")]
        public int? ShippingAddressId { get; set; }
        public virtual Address ShippingAddressFK { get; set; } = default!;
    }
}
