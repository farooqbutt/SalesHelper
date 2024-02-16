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
        public string? Comments { get; set; }


        // Quotation Document
        public List<IFormFile> DocumentFiles { get; set; } = default!;
    }
}
