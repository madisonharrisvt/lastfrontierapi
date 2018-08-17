using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class EventListController : Controller
    {
        private readonly LfContext _context;

        public EventListController(LfContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Event> GetAllEvents()
        {
            return _context.tblEvent.ToList();
        }
    }
}