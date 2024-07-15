namespace SalesHelper.Models.InterfaceModels
{
    public class CreateSaleInterface
    {
        public Sale SaleData { get; set; } = default!;
        public List<SalesCabinetItem> CabinetItems { get; set; } = default!;
    }
}
