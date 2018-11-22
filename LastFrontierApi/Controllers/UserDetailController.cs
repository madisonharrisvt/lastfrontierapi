using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class UserDetailController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IPlayerService _playerService;

        public UserDetailController(UserManager<AppUser> userManager, IMapper mapper,
            ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager,
            IPlayerService playerService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(int playerId)
        {
            var player = _appDbContext.tblPlayer.Include(p => p.Identity).FirstOrDefault(p => p.Id == playerId);

            if (player == null) return new BadRequestObjectResult($"Could not find player with id {playerId}");

            var user = await _userManager.FindByEmailAsync(player.Identity.Email);
            var role = await _userManager.GetRolesAsync(user);

            var playerObj = JObject.FromObject(player);
            playerObj.Add("role", role.FirstOrDefault());

            return Ok(playerObj);
        }

        [HttpPut]
        public async Task<ActionResult> Manage([FromBody] JObject player)
        {
            var playerToUpdate = _appDbContext.tblPlayer.FirstOrDefault(p => p.Id == (int)player["id"]);
            var volunteerPoints = 0;

            if (player.ContainsKey("volunteerPoints")) volunteerPoints = (int) player["volunteerPoints"];

            if (playerToUpdate == null) return BadRequest("Unable to find given player.");

            playerToUpdate.VolunteerPoints = volunteerPoints;

            var identity = player["identity"];

            var appUser = await _userManager.FindByIdAsync(playerToUpdate.IdentityId);
            appUser.FirstName = identity["firstName"].ToString();
            appUser.LastName = identity["lastName"].ToString();
            appUser.Email = identity["email"].ToString();
            appUser.UserName = identity["email"].ToString();

            var result = await _userManager.UpdateAsync(appUser);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
            }

            _appDbContext.Update(playerToUpdate);
            _appDbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCharacterById(int id)
        {
            try
            {
                var player = _appDbContext.tblPlayer.Include(p => p.Identity).FirstOrDefault(p => p.Id == id);
                _playerService.DeletePlayer(player);

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