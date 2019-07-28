using System.Linq;
using FluentValidation;

namespace LastFrontierApi.Models.Validations
{
  public class HackingPuzzleValidator : AbstractValidator<HackingPuzzle>
  {
    public HackingPuzzleValidator()
    {
      RuleFor(hp => hp.Rows).SetCollectionValidator(new HackingPuzzleRowValidator());
      RuleFor(hp => hp.Flag).NotEmpty().WithMessage("Code cannot be empty!");
      RuleFor(hp => hp.Rows).NotEmpty().WithMessage("Filler words are required!");
      RuleFor(hp => hp.Rows.Select(r => r.Word)).Must(r =>
        {
          var rows = r.ToList();
          return rows.Distinct().Count() == rows.Count();
        })
        .WithMessage("Cannot have duplicate filler words");
      RuleFor(hp => hp.Rows).Must(row => row.Select(r => r.IsAnswer).Any()).WithMessage("Answer is required!");
      RuleFor(hp => hp.Rows).Must(row => row.Select(r => r.IsAnswer).Count() > 1)
        .WithMessage("Cannot have more than one answer!");
    }
  }
}