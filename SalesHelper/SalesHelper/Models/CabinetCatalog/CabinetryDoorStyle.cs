using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models.CabinetCatalog
{
    public class CabinetryDoorStyle
    {
        public int Id { get; set; }
        public string DoorStyleName { get; set; } = string.Empty;
        public double Price { get; set; }

        [ForeignKey("CatalogItemIdFK")]
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItemIdFK { get; set; } = new CatalogItem();

        public double ModificationDepthFee { get; set; }
        public double AssembleFee { get; set; }
    }
}
