using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SalesHelper.Models
{
    public class CountertopMaterial
    {
        public enum Brands
        {
            Cambria,
            Silestone,
            Caesarstone,
            [EnumMember(Value = "MSI Quartz")]
            MSIQuartz,
            [EnumMember(Value = "LG Viatera")]
            LGViatera,
            Compac,
            Corian,
            [EnumMember(Value = "Pental Quartz")]
            PentalQuartz,
            Wilsonart,
            [EnumMember(Value = "Q Quartz")]
            QQuartz,
            Zodiaq,
            Vadara,
            Hanstone,
        }

        public enum Materials
        {
            Quartz,
            Granite,
            Marble,
            Wood,
            [EnumMember(Value = "Solid Surface")]
            SolidSurface,
            Laminates,
            Concrete,
            Glass,
        }

        public int Id { get; set; }
        [MaxLength(100)]
        public string Material { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? Brand { get; set; }
        [MaxLength(100)]
        public string? OtherBrand { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? Color { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? VendorName { get; set; } = string.Empty;
        public decimal? VendorRate { get; set; }
        public decimal? PriceQuote { get; set; }

        // Foreign Key
        [ForeignKey("CountertopQuotationIdFk")]
        public int CountertopQuotationId { get; set; }
        public CountertopQuotation CountertopQuotationIdFk { get; set; } = default!;
    }
}
