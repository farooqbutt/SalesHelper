using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class AccountBilling
    {
        [Key]
        public int BillingId { get; set; }

        [MaxLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [MaxLength(20)]
        public string CreditCardType { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string CreditCardNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string NameOnCard { get; set; } = string.Empty;

        public DateTime? ExpirationDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string CVV { get; set; } = string.Empty;

        [ForeignKey("AccountNumberFK")]
        public int AccountNumber { get; set; }
        public Account AccountNumberFK { get; set; } = default!; 

        [ForeignKey("BillingAddressFK")]
        public int? BillingAddressId { get; set; }
        public Address BillingAddress { get; set; } = default!;
    }
}
