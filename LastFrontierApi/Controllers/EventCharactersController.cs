using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class EventCharactersController : Controller
    {
        private readonly LfContext _context;

        public EventCharactersController(LfContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public ICollection<CharacterEvent> GetCharacterById(int id)
        {
            var characterEvents = _context.tblCharacterEvents.Include(ce => ce.Character).Where(c => c.EventId == id).ToList();

            return characterEvents;
        }

        [HttpPut]
        public IActionResult UpdateCharacterEventDetails([FromBody] CharacterEvent characterEvent)
        {
            var characterEventToUpdate = _context.tblCharacterEvents.FirstOrDefault(ce => ce.Id == characterEvent.Id);
            _context.Entry(characterEventToUpdate).CurrentValues.SetValues(characterEvent);

            _context.SaveChanges();

            return Ok();
        }
    }
}