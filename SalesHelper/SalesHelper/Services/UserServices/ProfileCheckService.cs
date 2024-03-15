using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;

namespace SalesHelper.Services.UserServices
{
    public class ProfileCheckService : IProfileCheckService
    {
        private readonly ApplicationDbContext _context;

        public ProfileCheckService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsProfileComplete(int accountNumber)
        {
            var userAccount = _context.Account.Where(a => a.AccountNumber == accountNumber)
                .Include(a => a.BusinessTypeFK)
                .Include(a => a.BillingAddress)
                .Include(a => a.ShippingAddress)
                .Include(a => a.BusinessAddress).FirstOrDefault();

            if (userAccount == null)
            {
                return false;
            }


            if (
                string.IsNullOrEmpty(userAccount.CompanyName) ||
                string.IsNullOrEmpty(userAccount.MainPhone) ||
                string.IsNullOrEmpty(userAccount.Fax) ||
                string.IsNullOrEmpty(userAccount.Email) ||
                string.IsNullOrEmpty(userAccount.BusinessAddress.Address1) ||
                string.IsNullOrEmpty(userAccount.BusinessAddress.Address2) ||
                string.IsNullOrEmpty(userAccount.BusinessAddress.City) ||
                string.IsNullOrEmpty(userAccount.BusinessAddress.State) ||
                string.IsNullOrEmpty(userAccount.BusinessAddress.Country) ||
                string.IsNullOrEmpty(userAccount.BusinessAddress.PostalCode)
                )
            {
                return false;
            }

            return true;
        }
    }
}
