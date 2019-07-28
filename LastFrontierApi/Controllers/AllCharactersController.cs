using System.Linq;
using System.Threading.Tasks;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
  [Authorize(Policy = "ApiUser", Roles = "Admin")]
  [Route("api/[controller]")]
  public class AllCharactersController : Controller
  {
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly LfContext _lfContext;
    private readonly UserManager<AppUser> _userManager;

    public AllCharactersController(UserManager<AppUser> userManager, LfContext lfContext,
      ApplicationDbContext appDbContext)
    {
      _userManager = userManager;
      _lfContext = lfContext;
      _applicationDbContext = appDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var allPlayers = await _userManager.GetUsersInRoleAsync("User");
      var allPlayerIds = _applicationDbContext.tblPlayer.ToList();

      var playersWithIdsAndNames = from player in allPlayers
        join playerId in allPlayerIds
          on player.Id equals playerId.IdentityId
        select new
        {
          playerId.Id,
          player.FirstName,
          player.LastName
        };

      var allCharacters = _lfContext.tblCharacter.ToList();

      var query = from player in playersWithIdsAndNames
        join character in allCharacters
          on player.Id equals character.PlayerId
        select new
        {
          playerId = player.Id,
          characterId = character.Id,
          firstName = player.FirstName,
          lastName = player.LastName,
          characterName = character.Name
        };


      return Ok(query);
    }
  }
}