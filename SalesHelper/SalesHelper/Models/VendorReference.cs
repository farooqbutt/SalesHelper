using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class VendorReference
    {
        [Key]
        public int VendorReferenceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Address1 { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Address2 { get; set; } = string.Empty;
         
        [MaxLength(100)]
        public string? City { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? State { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PostalCode { get; set; }  = string.Empty;

        [MaxLength(100)]
        public string? Country { get; set; }= string.Empty;

        [MaxLength(20)]
        public string? MainPhone { get; set; }= string.Empty;

        [MaxLength(20)]
        public string? Fax { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Website { get; set; }= string.Empty;

        [MaxLength(1024)]
        public string? Description { get; set; } =   string.Empty;

        [ForeignKey("BusinessTypeIdFK")]
        public int BusinessTypeId { get; set; }
        public BusinessTypes BusinessTypeIdFK { get; set; } = default!;
    }
}
