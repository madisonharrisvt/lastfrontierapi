using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class CheckInController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly ICharacterService _characterService;
        private readonly IEventService _eventService;

        public CheckInController(IPlayerService playerService, ICharacterService characterService, IEventService eventService)
        {
            _playerService = playerService;
            _characterService = characterService;
            _eventService = eventService;
        }

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

                //Email.SendEmail(newPlayerEmail, lfEvent);

                return new OkObjectResult(player.Id);

            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(checkInData);
            }
        }
    }
}