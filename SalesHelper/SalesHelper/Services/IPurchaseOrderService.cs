using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services
{
    public interface IPurchaseOrderService
    {
        public void Create(PurchaseOrder purchaseOrder);
        public PurchaseOrder Read(int id);
        public List<PurchaseOrder> ReadAll();
        public void Update(PurchaseOrder purchaseOrder);
        public void Delete(int id);
        
        // Other methods
        public int CreatePONumber(int accountId);

        public Task<List<POEmailInterface>> ReadPOEmails(int? poNumber, ApplicationUser user); // Read PO Emails
    }
}
