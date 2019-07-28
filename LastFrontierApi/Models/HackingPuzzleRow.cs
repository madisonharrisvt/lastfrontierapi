using FluentValidation.Attributes;
using LastFrontierApi.Models.Validations;

namespace LastFrontierApi.Models
{
  [Validator(typeof(HackingPuzzleRowValidator))]
  public class HackingPuzzleRow
  {
    public int Id { get; set; }
    public string Word { get; set; }
    public bool IsAnswer { get; set; }
    public int HackingPuzzleId { get; set; }
  }
}