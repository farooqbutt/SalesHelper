namespace SalesHelper.Models.InterfaceModels
{
    public class QuoteEmailsInterface
    {
        public int InboxUnReadCount { get; set; }
        public int QuoteId { get; set; }
        public string QuoteType { get; set; } = string.Empty;
        public string FolderName { get; set; } = string.Empty;
        public List<EmailMessage> Emails { get; set; } = new List<EmailMessage>();
    }

    public class EmailMessage
    {
        public string MessageId { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public bool IsStarred { get; set; }
        public bool HasAttachments { get; set; }
        public List<AttachmentModel> Attachments { get; set; } = new List<AttachmentModel>();
    }

    public class AttachmentModel
    {
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
    }
}
