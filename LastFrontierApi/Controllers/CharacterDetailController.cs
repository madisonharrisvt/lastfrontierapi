using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
        public Character GetCharacterById(Guid id)
        {
            Character character = _context.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == id);

            return character;
        }

        [HttpPut]
        public Character UpdateCharacterNameById([FromBody] Character character)
        {

            var characterToUpdate = _context.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == character.Id);

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
        public IActionResult DeleteCharacterById(Guid id)
        {
            var characterId = new SqlParameter("CharacterId", SqlDbType.UniqueIdentifier) { Value = id };
            var query = $"DELETE FROM tblCharacter WHERE id = @CharacterId";

            _context.Database.ExecuteSqlCommand(query, characterId);

            return Ok();
        }
    }
}
