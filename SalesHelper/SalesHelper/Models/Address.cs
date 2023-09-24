using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address1 { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Address2 { get; set; }= string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string AddressType { get; set; } = string.Empty;
        public DateTime CreatedDateTime { get; set; }

        public Address()
        {
            CreatedDateTime = DateTime.Now;
        }
    }
}
