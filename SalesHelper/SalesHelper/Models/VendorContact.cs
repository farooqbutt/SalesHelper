using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class VendorContact
    {
        [Key]
        public int VendorContactId { get; set; }

        [MaxLength(50)]
        public string ContactName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(20)]
        public string OfficePhone { get; set; } = string.Empty;

        [MaxLength(20)]
        public string OfficePhoneExtension { get; set; } = string.Empty;

        [MaxLength(20)]
        public string CellPhone { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Fax { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("VendorIdFK")]
        public int VendorId { get; set; }
        public Vendor VendorIdFK { get; set; } = default!;
    }
}
