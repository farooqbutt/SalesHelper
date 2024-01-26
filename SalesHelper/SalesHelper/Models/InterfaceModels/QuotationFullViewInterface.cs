namespace SalesHelper.Models.InterfaceModels
{
    public class QuotationFullViewInterface
    {
        public List<IFormFile> DocumentFiles { get; set; } = default!;
        public QuotationDocument QuotationDocument { get; set; } = default!;
        public CabinetQuotation CabinetQuotation { get; set; } = default!;
    }
}
