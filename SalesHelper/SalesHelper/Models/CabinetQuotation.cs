using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class CabinetQuotation 
    {
        public int Id { get; set; }

        [ForeignKey("CustomerIdFk")]
        public int CustomerId { get; set; }
        public Customer CustomerIdFk { get; set; } = default!;

        public string Name { get; set; } = string.Empty;

        [ForeignKey("VendorIdFk")]
        public int VendorId { get; set; }
        public Vendor VendorIdFk { get; set; } = default!;

        public string? Construction { get; set; } = string.Empty;
        public string? BoxMaterials { get; set; } = string.Empty;
        // is one color design or multiple color design
        public bool? isOneColorDesign { get; set; } // true for one color design, false for multiple color design
        //feilds for one color design
        public string? WoodSpeciesForOneColorDesign { get; set; } = string.Empty;
        public string? DoorStyleForOneColorDesign { get; set; } = string.Empty;
        public string? CabinetFinishForOneColorDesign { get; set; } = string.Empty;
        // Fields for multiple color design
        public string? DoorStyleForMultipleColorDesign { get; set; } = string.Empty;
        public string? UpperWoodSpeciesForMultipleColorDesign { get; set; } = string.Empty;
        public string? LowerWoodSpeciesForMultipleColorDesign { get; set; } = string.Empty;
        public string? IslandWoodSpeciesForMultipleColorDesign { get; set; } = string.Empty;
        public string? UpperFinishForMultipleColorDesign { get; set; } = string.Empty;
        public string? LowerFinishForMultipleColorDesign { get; set; } = string.Empty;
        public string? IslandFinishForMultipleColorDesign { get; set; } = string.Empty;
        public string? CommentsOnMultiColorDesign { get; set; } = string.Empty;

        // Additional Information for cabinet
        public string? AdditionalInformation { get; set; }

        // Fields for pricing
        public decimal? CabinetPrice { get; set; }
        public decimal? DeliveryCharge { get; set; }
        public decimal? InstallationFee { get; set; }
        public decimal? Tax { get; set; }
        public decimal? VendorPrice { get; set; } 
        public string? CommentOnPrice { get; set; } = string.Empty;
        // Created By 
        public string CreatedByUserId { get; set; } = string.Empty;

        // Fields for time tracking
        public DateTime? CreatedDateTime { get; set; } = new DateTime();
        public DateTime? ModifiedDateTime { get; set; } = new DateTime();
    }
}