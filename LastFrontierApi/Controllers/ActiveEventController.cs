using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin, User")]
    [Route("api/[controller]")]
    public class ActiveEventController : Controller
    {
        private readonly IEventService _eventService;

        public ActiveEventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public Event GetActiveEvent()
        {
            return _eventService.GetActiveEvent();
        }
    }
}