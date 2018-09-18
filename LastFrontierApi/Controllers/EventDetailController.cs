using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class EventDetailController : Controller
    {
        private readonly LfContext _context;

        public EventDetailController(LfContext context)
        {
            _context = context;
        }

        [HttpGet]
        public Event GetEventById(int eventId)
        {
            return _context.tblEvent.FirstOrDefault(e => e.Id == eventId);
        }

        [HttpPut]
        public Event UpdateOrCreateEvent([FromBody] Event lfEvent)
        {

            Event eventToUpdate = null;

            var lfEventStartTime = lfEvent.StartDate.TimeOfDay;
            lfEvent.StartDate = lfEvent.StartDate.Subtract(lfEventStartTime);

            var lfEventEndTime = lfEvent.EndDate.TimeOfDay;
            lfEvent.EndDate = lfEvent.EndDate.Subtract(lfEventEndTime);


            if (lfEvent.Id != 0)
            {
                eventToUpdate = _context.tblEvent.FirstOrDefault(e => e.Id == lfEvent.Id);
            }

            if (eventToUpdate != null)
            {
                _context.Entry(eventToUpdate).CurrentValues.SetValues(lfEvent);
            }
            else
            {
                _context.Add(lfEvent);
            }

            _context.SaveChanges();

            return lfEvent;
        }
    }
}