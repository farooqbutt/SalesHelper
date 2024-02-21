using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesHelper.Models;
using SalesHelper.Services;

namespace SalesHelper.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventService _eventService;
        private readonly CustomerService _customerService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public EventsController(
            SignInManager<ApplicationUser> signInManager,
            EventService eventService, 
            CustomerService customerRepo)
        {
            _eventService = eventService;
            _customerService = customerRepo;
            _signInManager = signInManager;
        }

        public IActionResult CalendarView()
        {
            return View();
        }

        public Event SetAccountNumAndCreatorOfEvent(Event eventObject)
        {
            eventObject.AccountNumber = _signInManager.UserManager.GetUserAsync(User).Result.AccountNumber;
            eventObject.CreatedByUserId = _signInManager.UserManager.GetUserAsync(User).Result.Id;
            return eventObject;
        }
        public IActionResult AddEvent(Event eventObject)
        {
            SetAccountNumAndCreatorOfEvent(eventObject);
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
            SetAccountNumAndCreatorOfEvent(eventObject);
            _eventService.Update(eventObject);
            return RedirectToAction("CalendarView");
        }

        [HttpPost]
        public string DeleteEvent(int id)
        {
            _eventService.Delete(id);
            return "Event Deleted Successfully!";
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
