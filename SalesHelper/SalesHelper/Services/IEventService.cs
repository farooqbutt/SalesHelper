using SalesHelper.Models;

namespace SalesHelper.Services
{
    public interface IEventService
    {
        public void Create(Event eventObj);
        public Event Read(int id);
        public List<Event> ReadAll();
        public void Update(Event eventObj);
        public void Delete(int id);
    }
}
