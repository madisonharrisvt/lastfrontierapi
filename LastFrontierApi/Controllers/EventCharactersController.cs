using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
  [Authorize(Policy = "ApiUser", Roles = "Admin")]
  [Route("api/[controller]")]
  public class EventCharactersController : Controller
  {
    private readonly ApplicationDbContext _appDbContext;
    private readonly LfContext _context;

    public EventCharactersController(LfContext context, ApplicationDbContext appDbContext)
    {
      _context = context;
      _appDbContext = appDbContext;
    }

    [HttpGet("{id}")]
    public IActionResult GetCharactersForEventId(int id)
    {
      var characterEvents =
        _context.tblCharacterEvents.Include(ce => ce.Character).Where(c => c.EventId == id).ToList();

      var characterEventsWithPlayers = new List<CharacterEventWithPlayer>();

      foreach (var characterEvent in characterEvents)
      {
        var player = _appDbContext.tblPlayer.Include(p => p.Identity)
          .FirstOrDefault(p => p.Id == characterEvent.Character.PlayerId);
        var playerJson = JObject.FromObject(player);

        var characterEventWithPlayer = new CharacterEventWithPlayer
        {
          CharacterEvent = characterEvent,
          Player = player
        };
        characterEventsWithPlayers.Add(characterEventWithPlayer);
      }

      return Ok(characterEventsWithPlayers);
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