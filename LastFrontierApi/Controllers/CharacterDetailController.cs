using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var query = $"SELECT * FROM tblCharacter WHERE id = @id ";
            var nameParam = new SqlParameter("id", SqlDbType.UniqueIdentifier) { Value = id };

            var character = _context.tblCharacter
                .FromSql(query, nameParam)
                .Select(c => new Character
                {
                    Name = c.Name,
                    Id = c.Id
                }).ToList().FirstOrDefault();

            return character;
        }

        [HttpPut]
        public Character UpdateCharacterNameById([FromBody] JObject body)
        {
            var name = body["name"].ToString();
            var id = new Guid(body["id"].ToString());

            var query = $"UPDATE tblCharacter SET name = @name WHERE id = @id; SELECT * FROM tblCharacter WHERE id = @id;";

            object[] listParams = {
                new SqlParameter("name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("id", SqlDbType.UniqueIdentifier) { Value = id }
            };

            var character = _context.tblCharacter
                .FromSql(query, listParams)
                .Select(c => new Character
                {
                    Name = c.Name,
                    Id = c.Id
                }).ToList().FirstOrDefault();

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
