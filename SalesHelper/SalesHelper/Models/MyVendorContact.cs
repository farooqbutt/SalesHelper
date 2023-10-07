using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class MyVendorContact
    {
        [Key]
        public int MyVendorContactId { get; set; }

        [ForeignKey("VendorContactIdFK")]
        public int VendorContactId { get; set; }
        public VendorContact VendorContactIdFK { get; set; } = default!;

        [ForeignKey("AccountNumberFK")]
        public int AccountNumber { get; set; }
        public Account AccountNumberFK { get; set; } = default!;
    }
}
