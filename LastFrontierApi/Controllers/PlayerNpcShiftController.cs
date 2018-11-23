using System;
using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class PlayerNpcShiftController : Controller
    {
        private readonly LfContext _context;
        private readonly ApplicationDbContext _appDbContext;

        public PlayerNpcShiftController(LfContext context, ApplicationDbContext appDbContext, IEventService eventService,
            IPlayerService playerService)
        {
            _context = context;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult GetPlayersWithoutNpcShiftForActiveEvent()
        {
            try
            {
                var charactersWithoutNpcShiftForActiveEvent = _context.Sp_GetPlayersWithoutNpcShiftForActiveEvent.FromSql("GetPlayersWithoutNpcShiftForActiveEvent").ToArray();

                var playersWithoutNpcShift = _appDbContext.tblPlayer.Include(p => p.Identity).Where(p => charactersWithoutNpcShiftForActiveEvent.FirstOrDefault(c => c.Id == p.Id) != null);

                if (!playersWithoutNpcShift.Any()) return BadRequest("All players for this event have an NPC shift!");

                return Ok(playersWithoutNpcShift);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                throw;
            }

            return Ok();
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

        [HttpDelete("{id}")]
        public IActionResult DeletePlayerNpcShift(int id)
        {
            try
            {
                var playerNpcShift = _context.tblPlayerNpcShifts.FirstOrDefault(p => p.Id == id);
                if (playerNpcShift == null) throw new Exception("Could not find PlayerNpcShift with id '" + id + "'!");
                _context.tblPlayerNpcShifts.Remove(playerNpcShift);
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