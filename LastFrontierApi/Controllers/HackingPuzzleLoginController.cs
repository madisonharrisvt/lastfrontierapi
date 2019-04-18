using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastFrontierApi.Extensions;
using LastFrontierApi.Models;
using LastFrontierApi.Models.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
  [Route("api/[controller]")]
  public class HackingPuzzleLoginController : Controller
  {
    private readonly LfContext _context;

    public HackingPuzzleLoginController(LfContext context)
    {
      _context = context;
    }

    [HttpGet("{id}")]
    public IActionResult Login(string id)
    {
      try
      {
        if (id == null) return BadRequest("Password cannot be null!");

        var validHackingPuzzle = _context.tblHackingPuzzle.FirstOrDefault(hp => hp.Password.Equals(id));

        if (validHackingPuzzle == null) return BadRequest("Invalid Password!");

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

    [HttpGet]
    public IActionResult GetAttempts()
    {
      try
      {
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}