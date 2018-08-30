using System.Linq;
using LastFrontierApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly LfContext _context;

        public CharacterService(LfContext context)
        {
            _context = context;
        }

        public Character UpdateOrCreateCharacter(Character character)
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
    }
}
