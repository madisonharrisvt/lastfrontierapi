using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin, User")]
    [Route("api/[controller]")]
    public class CharacterEventsController : Controller
    {
        private readonly LfContext _context;
        private readonly IEventService _eventService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _appDbContext;

        public CharacterEventsController(LfContext context, IEventService eventService, UserManager<AppUser> userManager, ApplicationDbContext appDbContext)
        {
            _context = context;
            _eventService = eventService;
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCharacterById(int id)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByEmailAsync(userName);
            var roles = await _userManager.GetRolesAsync(user);

            Character character;
            if (roles.Count == 1 && roles.FirstOrDefault() == "User")
            {
                var player = _appDbContext.tblPlayer.Include(p => p.Identity)
                    .FirstOrDefault(p => p.Identity.Equals(user));

                if (player == null) return BadRequest("Unable to find player");

                character = _context.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == id && c.PlayerId == player.Id);
            }
            else character = _context.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == id);

            if (character == null)
                return BadRequest("Unable to find given character.  Are you sure you have permission to view this data?");

            var characterEvents = _context.tblCharacterEvents.Where(c => c.CharacterId == character.Id).ToList();

            return Ok(characterEvents);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult AddCharacterToEvent([FromBody] JObject body)
        {
            var characterId = (int)body["characterId"];
            var eventId = (int)body["eventId"];

            _eventService.AddCharacterToEvent(characterId, eventId);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCharacterFromEvent(int id)
        {
            try
            {
                var characterEvent = _context.tblCharacterEvents.FirstOrDefault(c => c.Id == id);
                if (characterEvent == null) return BadRequest("Could not find character event with id '" + id + "'!");
                _context.tblCharacterEvents.Remove(characterEvent);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}