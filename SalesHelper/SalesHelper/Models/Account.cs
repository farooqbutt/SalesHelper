using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountNumber { get; set; }
        [MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;
        [MaxLength(20)]
        public string MainPhone { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Fax { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(100)]
        public string PrimaryContact { get; set; } = string.Empty;
        [MaxLength(10)]
        public string PrimaryContactPhone { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Website { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("BusinessTypeFK")]
        public int? BusinessTypeId { get; set; }
        public BusinessTypes BusinessTypeFK { get; set; } = default!;

        [ForeignKey("BusinessAddressFK")]
        public int? BusinessAddressId { get; set; }
        public Address BusinessAddress { get; set; } = default!;

        [ForeignKey("BillingAddressFK")]
        public int? BillingAddressId { get; set; }
        public Address BillingAddress { get; set; } = default!;

        [ForeignKey("ShippingAddressFK")]
        public int? ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; } = default!;

        public Account()
        {
            RegistrationDate = DateTime.Now;
            IsActive = true;
        }
    }
}
