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
    public class CharacterDetailController : Controller
    {
        private readonly LfContext _context;
        private readonly ICharacterService _characterService;

        public CharacterDetailController(LfContext context, ICharacterService characterService)
        {
            _context = context;
            _characterService = characterService;
        }

        [HttpGet("{id}")]
        public Character GetCharacterById(int id)
        {
            var character = _context.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == id);


            return character;
        }

        [HttpPut]
        public Character UpdateOrCreateCharacter([FromBody] Character character)
        {
            return _characterService.UpdateOrCreateCharacter(character);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCharacterById(int id)
        {
            var characterToDelete = new Character {Id = id};
            _context.tblCharacter.Attach(characterToDelete);
            _context.tblCharacter.Remove(characterToDelete);
            _context.SaveChanges();

            return Ok();
        }
    }
}
