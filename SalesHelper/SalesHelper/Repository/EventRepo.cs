using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public class EventRepo : IEventRepo
    {
        private readonly ApplicationDbContext _context;
        public EventRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Event eventObj)
        {
            try
            {
                _context.Events.Add(eventObj);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.Events.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Event Read(int id)
        {
            try
            {
                return _context.Events.Find(id)!;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<Event> ReadAll()
        {
            try
            {
                return _context.Events.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void Update(Event eventObj)
        {
            try
            {
                _context.Events.Update(eventObj);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
