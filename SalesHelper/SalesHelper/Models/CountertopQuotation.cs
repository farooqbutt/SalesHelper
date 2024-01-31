using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SalesHelper.Models
{
    public class CountertopQuotation
    {
        public enum RoomTypes
        {
            Kitchen,
            Bathroom,
            Fireplace,
            Backyard,
            [EnumMember(Value = "Wall Surface")]
            WallSurface,
            Other
        }

        public enum EdgeProfiles
        {
            Eased,
            Bevel,
            Ogee,
            Bullnose,
            [EnumMember(Value = "Full Bullnose")]
            FullBullnose,
            [EnumMember(Value = "Half Bullnose")]
            HalfBullnose,
            Waterfall,
            Pencil,
            Mitered
        }

        public int Id { get; set; }

        [ForeignKey("CustomerIdFk")]
        public int CustomerId { get; set; }
        public Customer CustomerIdFk { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? RoomType { get; set; }
        [MaxLength(100)]
        public string? EdgeProfile { get; set; }

        public bool MiteredEdge { get; set; } = false;
        public bool FarmhouseSink { get; set; } = false;
        public bool Cooktop { get; set; } = false;
        public bool FullBacksplash { get; set; } = false;
        public bool MiteredEdgeOnIsland { get; set; } = false;
        public bool IslandPrepSink { get; set; } = false;
        public bool Rangetop { get; set; } = false;
        public bool FourIncBacksplash { get; set; } = false;
        public bool WaterfallOnIsland { get; set; } = false;

        public string? EstimateSquareFeet { get; set; } = string.Empty;

        // Created By 
        public string CreatedByUserId { get; set; } = string.Empty;

        // Fields for time tracking
        public DateTime? CreatedDateTime { get; set; } = new DateTime();
        public DateTime? ModifiedDateTime { get; set; } = new DateTime();
    }

    public static class EnumExtensions
    {
        public static string GetEnumMemberAttributeValue(this Enum value)
        {
            var memberInfo = value.GetType().GetMember(value.ToString());
            var enumMemberAttribute = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false)
                                                  .FirstOrDefault() as EnumMemberAttribute;

            return enumMemberAttribute?.Value ?? value.ToString();
        }
    }
}
