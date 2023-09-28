using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesHelper.Models
{
    public class MyVendorContact
    {
        [Key]
        public int MyVendorContactId { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneExtention { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;



        [ForeignKey("VendorIdFK")]
        public int VendorId { get; set; }
        public Vendor VendorIdFK { get; set; } = default!;

        [ForeignKey("AccountNumberFK")]
        public int AccountNumber { get; set; }
        public Account AccountNumberFK { get; set; } = default!;
    }
}
