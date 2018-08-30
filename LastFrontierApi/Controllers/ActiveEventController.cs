using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class ActiveEventController : Controller
    {
        private readonly LfContext _context;

        public ActiveEventController(LfContext context)
        {
            _context = context;
        }

        [HttpGet]
        public Event GetActiveEvent()
        {
            return _context.tblEvent.FirstOrDefault(e => e.IsActiveEvent);
        }
    }
}