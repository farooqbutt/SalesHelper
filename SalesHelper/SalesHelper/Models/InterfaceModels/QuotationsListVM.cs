namespace SalesHelper.Models.InterfaceModels
{
    public class QuotationsListVM
    {
        public enum QuoteTypes
        {
            Cabinet,
            Countertop
        }
        public enum QuoteTypesIntials
        {
            CBNT, // Cabinet
            CNTP // Countertop
        }
        public string QuotationId { get; set; } = string.Empty;
        public string QuotationName { get; set; } = string.Empty;
        public Customer Customer { get; set; } = new Customer();
        public string QuoteType { get; set; } = string.Empty;
        public Vendor? Vendor { get; set; } = new Vendor();
        public DateTime? CreatedDate { get; set; } = new DateTime();
        public DateTime? ModifiedDate { get; set; } = new DateTime();
    }
}
