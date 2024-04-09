using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesHelper.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("AccountNumberFK")]
        public int AccountNumber { get; set; }
        public Account AccountNumberFK { get; set; } = default!;

        public ApplicationUser()
        {
            RegistrationDate = DateTime.Now;
            IsActive = true;
        }
    }
}
