using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class MyVendor
    {
        [Key]
        public int MyVendorId { get; set; }

        [ForeignKey("VendorIdFK")]
        public int VendorId { get; set; }
        public Vendor VendorIdFK { get; set; } = default!;

        [ForeignKey("AccountNumberFK")]
        public int AccountNumber { get; set; }
        public Account AccountNumberFK { get; set; } = default!; 
    }
}
