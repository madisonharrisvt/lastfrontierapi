using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LastFrontierApi.Extensions;
using LastFrontierApi.Models;
using LastFrontierApi.Models.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Controllers
{
  [Route("api/[controller]")]
  public class HackingPuzzleController : Controller
  {
    private readonly LfContext _context;

    public HackingPuzzleController(LfContext context)
    {
      _context = context;
    }

    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [HttpPut]
    public IActionResult GenerateHackingPuzzle([FromBody] HackingPuzzle hackingPuzzle)
    {
      try
      {
        _context.Database.ExecuteSqlCommand("TRUNCATE TABLE tblHackingPuzzleRow;");
        _context.Database.ExecuteSqlCommand("DELETE FROM tblHackingPuzzle;");

        var validator = new HackingPuzzleValidator();
        var result = validator.Validate(hackingPuzzle);

        if (!result.IsValid)
        {
          var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
          var errorMessages = string.Join(" ", errors);
          throw new Exception(errorMessages);
        }

        _context.tblHackingPuzzle.Add(hackingPuzzle);
        _context.tblHackingPuzzleRow.AddRange(hackingPuzzle.Rows);

        _context.SaveChanges();

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet]
    public IActionResult GetHackingPuzzle()
    {
      try
      {
        var hackingPuzzle = _context.tblHackingPuzzle.Include(hp => hp.Rows).FirstOrDefault();

        var symbols = new List<char>()
        {
          '!', '@', '#', '$', '%', '^', '&',
          '*', '(', ')', '-', '_', '+', '=',
          '{', '}', '[', ']', '\\', '|', ':',
          ';', '"', '\'', '<', '>', ',', '.',
          '?', '/', '~', '`'
        };

        if (!hackingPuzzle.Rows.Any()) { throw new Exception("Hacking Puzzle has no rows!"); }

        var rows = hackingPuzzle.Rows.Select(r => r.Word).ToList();

        rows.Shuffle();

        var puzzleCodes = new Dictionary<string, string>();
        var random = new Random();
        foreach (var code in rows)
        {
          var codeLength = code.Length;
          var codeStart = random.Next(12 - codeLength);
          var encryptedCode = new StringBuilder();
          encryptedCode.Append(code);
          for (var j = 0; j < 12; j++)
          {
            var randomIndex = random.Next(symbols.Count);
            var randomChar = symbols[randomIndex];

            if (j < codeStart)
            {
              encryptedCode.Insert(j, randomChar);
            }
            else if (j >= codeStart + codeLength)
            {
              encryptedCode.Append(randomChar);
            }
          }
          puzzleCodes.Add(code, encryptedCode.ToString());
        }

        return Ok(puzzleCodes);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}