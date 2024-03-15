namespace SalesHelper.Models.EmailModels
{
    public class MailSettings
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string IMAP_Server { get; set; } = string.Empty;
        public int IMAP_Port { get; set; }
        public string IMAP_Encryption { get; set; } = string.Empty;
        public string SMTP_Server { get; set; } = string.Empty;
        public int SMTP_Port { get; set; }
        public string SMTP_Encryption { get; set; } = string.Empty;
    }
}
