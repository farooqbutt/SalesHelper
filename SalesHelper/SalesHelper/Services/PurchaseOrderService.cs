using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;
        public PurchaseOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.ModifiedDate = DateTime.Now;
                _context.PurchaseOrders.Add(purchaseOrder);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.PurchaseOrders.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public PurchaseOrder Read(int id)
        {
            try
            {
                return _context.PurchaseOrders.Where(a => a.Id == id)
                    .Include(a => a.SalesOrderFK)
                    .Include(a => a.CustomerFK)
                    .Include(a => a.VendorFK)
                    .FirstOrDefault()!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<PurchaseOrder> ReadAll()
        {
            try
            {
                return _context.PurchaseOrders
                    .Include(a => a.SalesOrderFK)
                    .Include(a => a.CustomerFK)
                    .Include(a => a.VendorFK)
                    .OrderByDescending(a => a.OrderDate).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.ModifiedDate = DateTime.Now;
                _context.PurchaseOrders.Update(purchaseOrder);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int CreatePONumber(int accountId)
        {
            try
            {
                // get last PO number
                var lastPO = _context.PurchaseOrders.OrderByDescending(a => a.Id).Where(a => a.PurchaseOrderNumber != null).LastOrDefault();
                if (lastPO == null)
                {
                    return 10001;
                }
                else
                {
                    return lastPO!.PurchaseOrderNumber!.Value + 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
