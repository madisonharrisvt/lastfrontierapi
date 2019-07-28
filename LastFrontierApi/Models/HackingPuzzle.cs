using System.Collections.Generic;
using FluentValidation.Attributes;
using LastFrontierApi.Models.Validations;

namespace LastFrontierApi.Models
{
  [Validator(typeof(HackingPuzzleValidator))]
  public class HackingPuzzle
  {
    public int Id { get; set; }
    public string Flag { get; set; }
    public string Password { get; set; }
    public int Attempts { get; set; }
    public int AttemptsRemaining { get; set; }

    public virtual ICollection<HackingPuzzleRow> Rows { get; set; }
  }
}