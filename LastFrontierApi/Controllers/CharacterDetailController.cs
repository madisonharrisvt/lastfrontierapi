using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin, User")]
    [Route("api/[controller]")]
    public class CharacterDetailController : Controller
    {
        private readonly LfContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICharacterService _characterService;
        private readonly ApplicationDbContext _appDbContext;

        public CharacterDetailController(LfContext context, ICharacterService characterService, LfContext lfContext, UserManager<AppUser> userManager, ApplicationDbContext appDbContext)
        {
            _context = context;
            _characterService = characterService;
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

            return Ok(character);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public Character UpdateOrCreateCharacter([FromBody] Character character)
        {
            return _characterService.UpdateOrCreateCharacter(character);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCharacterById(int id)
        {
            var characterToDelete = _context.tblCharacter.Include(c => c.CartItems).FirstOrDefault(c => c.Id == id);

            if (characterToDelete == null) return BadRequest("Unable to find character with ID: " + id);

            _context.tblCharacter.Attach(characterToDelete);
            _context.tblCharacter.Remove(characterToDelete);
            _context.SaveChanges();

            return Ok();
        }
    }
}
