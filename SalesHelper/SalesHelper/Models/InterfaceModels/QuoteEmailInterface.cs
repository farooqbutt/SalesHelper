namespace SalesHelper.Models.InterfaceModels
{
    public class QuoteEmailInterface
    {
        public string MessageId { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<QuoteAttachmentModel> Attachments { get; set; } = new List<QuoteAttachmentModel>();
    }

    public class QuoteAttachmentModel
    {
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
    }
}
