using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class QuotationDocument
    {
        [Key]
        public int DocumentId { get; set; }
        [MaxLength(100)]
        public string? DocumentName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Description { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }

        [ForeignKey("QuotationIdFK")]
        public int QuotationId { get; set; }
        public CabinetQuotation QuotationIdFK { get; set; } = new CabinetQuotation();
    }
}
