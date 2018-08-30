using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class CharacterEventsController : Controller
    {
        private readonly LfContext _context;
        private readonly IEventService _eventService;

        public CharacterEventsController(LfContext context, IEventService eventService)
        {
            _context = context;
            _eventService = eventService;
        }

        [HttpGet("{id}")]
        public ICollection<CharacterEvent> GetCharacterById(int id)
        {
            var characterEvents = _context.tblCharacterEvents.Where(c => c.CharacterId == id).ToList();

            return characterEvents;
        }

        [HttpPut]
        public IActionResult AddCharacterToEvent([FromBody] JObject body)
        {
            var characterId = (int)body["characterId"];
            var eventId = (int)body["eventId"];

            _eventService.AddCharacterToEvent(characterId, eventId);

            return Ok();
        }
    }
}