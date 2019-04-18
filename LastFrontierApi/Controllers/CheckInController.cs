using System;
using System.Linq;
using System.Threading.Tasks;
using LastFrontierApi.Extensions;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;

namespace LastFrontierApi.Controllers
{
  [Authorize(Policy = "ApiUser", Roles = "User")]
  [Route("api/[controller]")]
  public class CheckInController : Controller
  {
    private readonly ICharacterService _characterService;
    private readonly IEventService _eventService;
    private readonly IPlayerService _playerService;
    private readonly ApplicationDbContext _appDbContext;
    private readonly LfContext _lfContext;

    public CheckInController(IPlayerService playerService, ICharacterService characterService,
      IEventService eventService, ApplicationDbContext appDbContext, LfContext lfContext)
    {
      _playerService = playerService;
      _characterService = characterService;
      _eventService = eventService;
      _appDbContext = appDbContext;
      _lfContext = lfContext;
    }

    [HttpPut]
    public IActionResult CheckIn([FromBody] Cart cart)
    {
      try
      {
        cart = cart.ThrowIfNull("Cart");
        var player = _appDbContext.tblPlayer.FirstOrDefault(p => p.Id == cart.PlayerId).ThrowIfNull("Player");
        var currentEvent = _lfContext.tblEvent.FirstOrDefault(e => e.IsActiveEvent).ThrowIfNull("CurrentEvent");
        var cartItems = cart.CartItems.ThrowIfNull("CartItems");

        foreach (var cartItem in cart.CartItems)
        {
          var characterTotalXp = 0;
          characterTotalXp += cartItem.PurchaseXp;
          characterTotalXp += cartItem.VpToXp;

          var character = _lfContext.tblCharacter.FirstOrDefault(c => c.Id == cartItem.CharacterId).ThrowIfNull($"Character with ID '{cartItem.CharacterId}'");

          if (_lfContext.tblCharacterEvents.FirstOrDefault(ce =>
                ce.CharacterId == character.Id && ce.EventId == currentEvent.Id) != null)
          {
            return BadRequest($"Character '{character.Name}' has already been registered for this event! Please remove.");
          }

          if (character.AccumulatedXP == null) character.AccumulatedXP = 0;
          if (character.AvailableXP == null) character.AvailableXP = 0;

          var baseXp = 40;

          characterTotalXp += baseXp;
          character.AccumulatedXP += characterTotalXp;
          character.AvailableXP += characterTotalXp;

          var newCharacterEvent = new CharacterEvent
          {
            VpToXp = cartItem.VpToXp,
            XpBought = cartItem.PurchaseXp,
            EventId = currentEvent.Id,
            CharacterId = cartItem.CharacterId
          };

          _lfContext.tblCharacter.Update(character);
          _lfContext.tblCharacterEvents.Add(newCharacterEvent);
        }

        _lfContext.SaveChanges();

        var totalVp = cartItems.Sum(ci => ci.VpToXp);

        player.VolunteerPoints = player.VolunteerPoints - totalVp;
        _appDbContext.tblPlayer.Update(player);
        _appDbContext.SaveChanges();

        return Ok(player.Id);
      }
      catch (Exception e)
      {
        return BadRequest(e);
      }
    }

    /*
    [HttpPut]
    public async Task<IActionResult> CheckIn([FromBody] CheckInData checkInData)
    {

      var newPlayerEmail = checkInData.NewPlayerEmail;
      var newCharacter = checkInData.NewCharacter;
      var lfEvent = checkInData.Event;

      try
      {
        var player = await _playerService.CreatePlayerFromEmail(newPlayerEmail);

        newCharacter.PlayerId = player.Id;

        var character = _characterService.UpdateOrCreateCharacter(newCharacter);

        _eventService.AddCharacterToEvent(character.Id, lfEvent.Id);

        Email.SendCheckInEmail(newPlayerEmail, lfEvent);

        return new OkObjectResult(player.Id);
      }
      catch (Exception e)
      {
        return new BadRequestObjectResult(checkInData);
      }
    }
    */
  }
}