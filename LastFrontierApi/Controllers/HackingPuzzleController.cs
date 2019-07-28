using System;
using System.Linq;
using LastFrontierApi.Models;
using LastFrontierApi.Models.Validations;
using Microsoft.AspNetCore.Authorization;
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

        hackingPuzzle.AttemptsRemaining = 0;

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
    public IActionResult ResetAttempts()
    {
      try
      {
        var hackingPuzzle = _context.tblHackingPuzzle.Include(hp => hp.Rows).FirstOrDefault();

        if (hackingPuzzle == null) throw new Exception("HackingPuzzle cannot be null!");

        hackingPuzzle.AttemptsRemaining = hackingPuzzle.Attempts;

        _context.tblHackingPuzzle.Update(hackingPuzzle);
        _context.SaveChanges();

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}