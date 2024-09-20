namespace SalesHelper.Models.InterfaceModels
{
    public class CustomerEmailInterface
    {
        public string MessageId { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<CustomerAttachmentModel> Attachments { get; set; } = new List<CustomerAttachmentModel>();
    }

    public class CustomerAttachmentModel
    {
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
    }
}
