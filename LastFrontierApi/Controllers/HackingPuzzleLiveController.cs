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
  public class HackingPuzzleLiveController : Controller
  {
    private readonly LfContext _context;

    public HackingPuzzleLiveController(LfContext context)
    {
      _context = context;
    }

    [HttpPut]
    public IActionResult SubmitAnswer([FromBody] JObject submission)
    {
      try
      {
        var symbols = new List<char>()
        {
          '!', '@', '#', '$', '%', '^', '&',
          '*', '(', ')', '-', '_', '+', '=',
          '{', '}', '[', ']', '\\', '|', ':',
          ';', '"', '\'', '<', '>', ',', '.',
          '?', '/', '~', '`'
        };

        var code = submission["code"].ToString();
        var wordSelected = code;
        foreach (var n in code)
        {
          if (symbols.Contains(n)) wordSelected = wordSelected.Replace($"{n}", "");
        }

        var hackingPuzzle = _context.tblHackingPuzzle.Include(hp => hp.Rows).FirstOrDefault();

        if (hackingPuzzle == null)
        {
          throw new Exception("HackingPuzzle cannot be null!");
        }

        if (hackingPuzzle.AttemptsRemaining <= 0)
        {
          var outOfAttempts = new JObject
          {
            { "OutOfAttempts", true }
          };
        }

        var answerRow = hackingPuzzle.Rows.FirstOrDefault(r => r.IsAnswer);

        if (answerRow == null)
        {
          throw new Exception("There is no answer! Well this puzzle sucks...");
        }

        var answer = answerRow.Word;

        if (answer.Equals(wordSelected))
        {
          var solution = new JObject
          {
            { "Flag", hackingPuzzle.Flag }
          };

          hackingPuzzle.AttemptsRemaining = hackingPuzzle.Attempts;
          _context.tblHackingPuzzle.Update(hackingPuzzle);

          return Ok(solution);
        }

        if (hackingPuzzle.AttemptsRemaining == 1)
        {
          var outOfAttempts = new JObject
          {
            { "OutOfAttempts", true }
          };

          hackingPuzzle.AttemptsRemaining--;
          _context.tblHackingPuzzle.Update(hackingPuzzle);
          _context.SaveChanges();

          return Ok(outOfAttempts);
        }

        var numberCorrect = 0;
        for (int i = 0; i < answer.Length; i++)
        {
          if (answer[i] == wordSelected[i])
          {
            numberCorrect++;
          }
        }

        var result = new JObject
        {
          { "NumberCorrect", numberCorrect },
          { "WordLength", answer.Length }
        };

        hackingPuzzle.AttemptsRemaining--;
        _context.tblHackingPuzzle.Update(hackingPuzzle);
        _context.SaveChanges();

        return Ok(result);
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
        var hackingPuzzle = _context.tblHackingPuzzle.ToList().FirstOrDefault();

        if (hackingPuzzle == null)
        {
          throw new Exception("HackingPuzzle cannot be null!");
        }

        var attempts = hackingPuzzle.Attempts;
        var attemptsRemaining = hackingPuzzle.AttemptsRemaining;

        var attemptsObj = new JObject
        {
          { "Attempts", attempts },
          { "AttemptsRemaining", attemptsRemaining }
        };

        return Ok(attemptsObj);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}