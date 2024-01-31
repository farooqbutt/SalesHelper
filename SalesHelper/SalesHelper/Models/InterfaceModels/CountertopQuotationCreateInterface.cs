namespace SalesHelper.Models.InterfaceModels
{
    public class CountertopQuotationCreateInterface
    {
        public CountertopQuotation CountertopQuotation { get; set; }  = new CountertopQuotation();
        public List<CountertopMaterial> CountertopMaterials { get; set; } = new List<CountertopMaterial>();
    }
}
