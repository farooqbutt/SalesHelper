using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class BusinessTypes
    {
        [Key]
        public int BusinessTypeId { get; set; }

        [Required, MaxLength(50)]
        public string TypeName { get; set; } = string.Empty;
    }
}
