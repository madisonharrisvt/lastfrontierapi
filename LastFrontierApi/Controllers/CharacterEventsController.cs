using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
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

        public CharacterEventsController(LfContext context)
        {
            _context = context;
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
            var characterId = body["characterId"];
            var eventId = body["eventId"];

            var newCharacterEvent = new CharacterEvent
            {
                CharacterId = (int)characterId,
                EventId = (int)eventId
            };

            _context.Add(newCharacterEvent);
            _context.SaveChanges();

            return Ok();
        }
    }
}