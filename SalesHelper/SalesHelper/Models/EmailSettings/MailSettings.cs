using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models.EmailSettings
{
    public class MailSettings
    {
        public int Id { get; set; }
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; }

        [ForeignKey("CreatedByUserIdFk")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser CreatedByUserIdFk { get; set; } = default!;
    }
}
