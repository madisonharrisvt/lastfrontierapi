using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin, User")]
    [Route("api/[controller]")]
    public class CharacterListController : Controller
    {
        private readonly LfContext _context;

        public CharacterListController(LfContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Character> GetCharactersByPlayerId(int playerId)
        {
            var characters = _context.tblCharacter.Include(c => c.Events).Where(c => c.PlayerId == playerId).ToList();
            return characters;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public Character Post([FromBody]JObject body)
        {
            var name = body["name"].ToString();
            var query = $@" DECLARE @newId varchar(255) = NEWID ()  INSERT INTO tblCharacter (id, name) VALUES(@newId, @name)  SELECT * from tblCharacter WHERE id = @newId";
            var nameParam = new SqlParameter("name", SqlDbType.NVarChar) { Value = name };

            var character = _context.tblCharacter
                .FromSql(query, nameParam)
                .Select(c => new Character
                {
                    Name = c.Name,
                    Id = c.Id
                }).ToList().FirstOrDefault();


            return character;
        }
    }
}
