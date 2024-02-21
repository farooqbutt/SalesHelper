using SalesHelper.Data;
using SalesHelper.Models;

namespace SalesHelper.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        public EventService(ApplicationDbContext context)
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
            }
        }
    }
}
