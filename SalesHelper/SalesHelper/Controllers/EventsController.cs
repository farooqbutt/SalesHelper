using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Repository;

namespace SalesHelper.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventRepo _eventService;
        public EventsController(EventRepo eventService)
        {
            _eventService = eventService;
        }

        public IActionResult CalendarView(int id)
        {
            TempData["CustomerId"] = id;
            return View();
        }

        public IActionResult AddEvent(Event eventObject)
        {   
            _eventService.Create(eventObject);
            return RedirectToAction("CalendarView");
        }

        [HttpGet]
        public JsonResult GetEvents()
        {
            var events = _eventService.ReadAll().Where(
                e => Convert.ToDateTime(e.Start).Month == DateTime.Now.Month &&
                Convert.ToDateTime(e.Start).Year == DateTime.Now.Year).ToList();
            return Json(events);
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
    }
}
