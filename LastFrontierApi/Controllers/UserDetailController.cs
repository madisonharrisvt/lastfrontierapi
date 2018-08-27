using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UserDetailController(UserManager<AppUser> userManager, IMapper mapper,
            ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult GetUserById(int playerId)
        {
            var player = _appDbContext.tblPlayer.Include(p => p.Identity).FirstOrDefault(p => p.Id == playerId);

            return Ok(player);
        }

        [HttpPut]
        public async Task<ActionResult> Manage([FromBody] JObject player)
        {
            var playerToUpdate = _appDbContext.tblPlayer.FirstOrDefault(p => p.Id == (int)player["id"]);

            var identity = player["identity"];

            if (playerToUpdate == null) return new BadRequestObjectResult(player);

            var appUser = await _userManager.FindByIdAsync(playerToUpdate.IdentityId);
            appUser.FirstName = identity["firstName"].ToString();
            appUser.LastName = identity["lastName"].ToString();
            appUser.Email = identity["email"].ToString();
            appUser.UserName = identity["email"].ToString();

            var result = await _userManager.UpdateAsync(appUser);

            if (!result.Succeeded) { return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState)); }

            return Ok();

        }
    }
}