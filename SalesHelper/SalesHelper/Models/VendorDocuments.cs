using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class VendorDocuments
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }

        [ForeignKey("VendorId")]
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; } = default!;
    }
}
