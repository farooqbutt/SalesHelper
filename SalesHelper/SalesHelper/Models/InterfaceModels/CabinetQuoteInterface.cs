namespace SalesHelper.Models.InterfaceModels
{
    public class CabinetQuoteInterface : CabinetQuotation
    {
        public string? Refrigerator { get; set; }
        public string? StoveAndCooktop { get; set; }
        public string? Dishwasher { get; set; }
        public string? Hood { get; set; }
        public string? BuiltInOven { get; set; }
        public string? BuiltInDrawerMicrowave { get; set; }
        public string? Sink { get; set; }
        public string? CommentsOnAdditionalInfo { get; set; }

        // Cabinet Details
        public List<CabinetDetail> CabinetDetailsList { get; set; } = default!;

        // Quotation Document
        public List<IFormFile> DocumentFiles { get; set; } = default!;

        // Quotation Email
        public List<QuoteEmailInterface> QuoteEmails { get; set; } = default!;
    }
}
