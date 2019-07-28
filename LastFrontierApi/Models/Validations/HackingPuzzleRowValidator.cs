using FluentValidation;

namespace LastFrontierApi.Models.Validations
{
  public class HackingPuzzleRowValidator : AbstractValidator<HackingPuzzleRow>
  {
    public HackingPuzzleRowValidator()
    {
      RuleFor(hpr => hpr.Word).NotEmpty().WithMessage("Words cannot be empty!");
      RuleFor(hpr => hpr.Word.Length).LessThanOrEqualTo(12).WithMessage("Words cannot be longer than 12 characters!");
    }
  }
}