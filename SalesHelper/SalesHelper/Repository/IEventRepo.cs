using SalesHelper.Models;

namespace SalesHelper.Repository
{
    public interface IEventRepo
    {
        public void Create(Event eventObj);
        public Event Read(int id);
        public List<Event> ReadAll();
        public void Update(Event eventObj);
        public void Delete(int id);
    }
}
