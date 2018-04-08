using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class CharacterSearchController : Controller
    {
        private readonly LfContext _context;

        public CharacterSearchController(LfContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IEnumerable<Character> GetByPartialName(string id)
        {
            var query = $"SELECT * FROM tblCharacter WHERE name LIKE '%' + @partialName + '%' ";

            var partialNameParam = new SqlParameter("partialName", SqlDbType.NVarChar) { Value = id };

            var characters = _context.tblCharacter
                .FromSql(query, partialNameParam)
                .Select(c => new Character
                {
                    Name = c.Name,
                    Id = c.Id
                }).ToList();

            return characters;
        }
    }
}
