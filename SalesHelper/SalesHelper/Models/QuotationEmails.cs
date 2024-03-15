using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class QuotationEmails
    {
        public int Id { get; set; }
        public string QuoteType { get; set; } = string.Empty;
        public int QuotationId { get; set; }

        [ForeignKey("UserIdFk")]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser UserIdFk { get; set; } = default!;

        public string EmailMessageId { get; set; } = string.Empty;
    }
}
