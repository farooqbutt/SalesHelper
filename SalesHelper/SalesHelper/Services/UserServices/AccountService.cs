using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services.UserServices
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Account Read(int id)
        {
            try
            {
                return _context.Account.Where(a => a.AccountNumber == id)
                    .Include(a => a.BusinessTypeFK)
                    .Include(a => a.BillingAddress)
                    .Include(a => a.ShippingAddress)
                    .Include(a => a.BusinessAddress)
                    .FirstOrDefault()!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
