using System;
using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class NpcShiftController : Controller
    {
        private readonly LfContext _context;
        private readonly IEventService _eventService;
        private readonly IPlayerService _playerService;

        public NpcShiftController(LfContext context, IEventService eventService,
            IPlayerService playerService)
        {
            _context = context;
            _eventService = eventService;
            _playerService = playerService;
        }

        [HttpGet]
        public IActionResult GetCurrentEventNpcShifts()
        {
            var npcShifts = _context.tblNpcShift.Include(n => n.Event).Where(n => n.Event.IsActiveEvent).ToList();

            if (!npcShifts.Any())
            {
                var activeEvent = _eventService.GetActiveEvent();
                var npcShiftBegins = activeEvent.StartDate.AddDays(1).AddHours(8);
                var npcShiftsEnds = activeEvent.EndDate.AddHours(2).AddMinutes(30);

                npcShifts = NpcShiftGenerator.GenerateShifts(activeEvent, npcShiftBegins, npcShiftsEnds, 150, 60);

                _context.tblNpcShift.AddRange(npcShifts);
                _context.SaveChanges();
                return Ok(npcShifts);
            }

            List<NpcShiftWithCountAndPlayers> npcShiftsWithCountsAndPlayers;
            try
            {
                var npcShiftsWithCounts = _context.Sp_GetNpcShiftsWithPlayerCount.FromSql("GetNpcShiftsWithPlayerCount").ToList();

                npcShiftsWithCountsAndPlayers = new List<NpcShiftWithCountAndPlayers>();
                foreach (var npcShift in npcShiftsWithCounts)
                {
                    var players = _playerService.GetPlayersFromNpcShift(npcShift.Id);
                    var npcShiftWithCountAndPlayers = new NpcShiftWithCountAndPlayers()
                    {
                        EventId =  npcShift.EventId,
                        Id = npcShift.Id,
                        StartDateTime = npcShift.StartDateTime,
                        EndDateTime = npcShift.EndDateTime,
                        NpcCount = npcShift.NpcCount,
                        Players = players
                    };

                    npcShiftsWithCountsAndPlayers.Add(npcShiftWithCountAndPlayers);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Ok(npcShiftsWithCountsAndPlayers);
        }

        [HttpPut]
        public IActionResult AddPlayerToNpcShift([FromBody] PlayerNpcShifts playerNpcShift)
        {
            try
            {
                var playerNpcShiftToAdd = new PlayerNpcShifts()
                {
                    PlayerId = playerNpcShift.PlayerId,
                    NpcShiftId = playerNpcShift.NpcShiftId
                };

                _context.tblPlayerNpcShifts.Add(playerNpcShiftToAdd);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Ok();
        }
    }
}