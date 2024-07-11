using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SalesHelper.Models
{
    public class CabinetDetail
    {
        public enum SectionNames
        {
            Upper,
            Lower,
            Island,
            [EnumMember(Value = "Entire Kitchen")]
            EntireKitchen
        }
        public enum Constructions 
        {
            Frameless,
            [EnumMember(Value = "Framed Inset")]
            FramedInset,
            [EnumMember(Value = "Framed Overlay")]
            FramedOverlay,
            [EnumMember(Value = "Framed Partial Overlay")]
            FramedPartialOverlay
        }
        public enum BoxMaterials
        {
            [EnumMember(Value = "1/2\" Plywood")]
            HalfInchPlywood,
            [EnumMember(Value = "3/4\" Plywood")]
            ThreeQuarterInchPlywood,
            MDF,
            [EnumMember(Value = "Particle Board")]
            ParticleBoard
        }
        public enum WoodSpecies
        {
            Alder,
            Birch,
            Cherry,
            Hickory,
            Maple,
            MDF,
            Pine,
            [EnumMember(Value = "Quarter Saw Red Oak")]
            QuarterSawRedOak,
            [EnumMember(Value = "Quarter Saw White Oak")]
            QuarterSawWhiteOak,
            [EnumMember(Value = "Red Oak")]
            RedOak,
            [EnumMember(Value = "Rift Cut Red Oak")]
            RiftCutRedOak,
            [EnumMember(Value = "Rift Cut White Oak")]
            RiftCutWhiteOak,
            [EnumMember(Value = "White Oak")]
            WhiteOak,
            Walnut
        }

        [Key]
        public int Id { get; set; }
        public string SectionName { get; set; } = string.Empty;

        [ForeignKey("VendorIdFk")]
        public int VendorId { get; set; }
        public Vendor VendorIdFk { get; set; } = default!;

        public string? Construction { get; set; } = string.Empty;
        public string? BoxMaterial { get; set; } = string.Empty;
        public string? WoodSpecie { get; set; }
        public string? DoorStyle { get; set; } = string.Empty;
        public string? CabinetFinish { get; set; } = string.Empty;

        [ForeignKey("CabinetQuotationIdFk")]
        public int CabinetQuotationId { get; set; }
        public CabinetQuotation CabinetQuotationIdFk { get; set; } = default!;
    }
}
