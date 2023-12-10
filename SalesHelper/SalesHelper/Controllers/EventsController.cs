using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Repository;

namespace SalesHelper.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventRepo _eventService;
        private readonly CustomerRepo _customerService;
        public EventsController(EventRepo eventService, CustomerRepo customerRepo)
        {
            _eventService = eventService;
            _customerService = customerRepo;
        }

        public IActionResult CalendarView()
        {
            return View();
        }

        public IActionResult AddEvent(Event eventObject)
        {
            _eventService.Create(eventObject);
            return RedirectToAction("CalendarView");
        }

        [HttpGet]
        public JsonResult GetEvents(int? id, DateTime startDate, DateTime endDate)
        {
            if (id == null)
            {
                var events = _eventService.ReadAll().Where(
                    e => Convert.ToDateTime(e.Start) >= startDate &&
                    Convert.ToDateTime(e.Start) <= endDate).ToList();
                foreach (var item in events)
                {
                    item.CustomerIdFK = _customerService.Read(item.CustomerId);
                }
                return Json(events);
            }
            else
            {
                var events = _eventService.Read((int)id);
                events.CustomerIdFK = _customerService.Read(events.CustomerId);
                return Json(events);
            }
        }

        [HttpPost]
        public IActionResult UpdateEvent(Event eventObject)
        {
            _eventService.Update(eventObject);
            return RedirectToAction("CalendarView");
        }

        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            _eventService.Delete(id);
            return RedirectToAction("CalendarView");
        }

        [HttpPost]
        public string ChangeEventStart(int id, string newStart)
        {
            var eventToChange = _eventService.Read(id);
            eventToChange.Start = newStart;
            _eventService.Update(eventToChange);
            return "Event Updated Successfully";
        }
    }
}
