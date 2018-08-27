using System.Linq;
using LastFrontierApi.Models;
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

        public CharacterDetailController(LfContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public Character GetCharacterById(int id)
        {
            //var character = _context.tblCharacter.Include(s => s.Skills).Include(e => e.Events).FirstOrDefault(c => c.Id == id);
            var character = _context.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == id);


            return character;
        }

        [HttpPut]
        public Character UpdateOrCreateCharacterById([FromBody] Character character)
        {
            Character characterToUpdate = null;

            if (character.Id != 0)
            {
                characterToUpdate = _context.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == character.Id);
            }

            if (characterToUpdate != null)
            {
                foreach (var skill in characterToUpdate.Skills)
                {
                    _context.tblCharacterSkills.Remove(skill);
                }

                _context.Entry(characterToUpdate).CurrentValues.SetValues(character);
            }
            else
            {
                _context.Add(character);
            }

            _context.tblCharacterSkills.AddRange(character.Skills);
            _context.SaveChanges();
            return character;
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
