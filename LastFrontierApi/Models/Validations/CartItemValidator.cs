using FluentValidation;

namespace LastFrontierApi.Models.Validations
{
  public class CartItemValidator : AbstractValidator<CartItem>
  {
    public CartItemValidator()
    {
      RuleFor(vm => vm.PurchaseXp).LessThanOrEqualTo(20).WithMessage("Purchased XP must be less than or equal to 20");
      RuleFor(vm => vm.VpToXp).LessThanOrEqualTo(20).WithMessage("VP to XP must be less than or equal to 20");
    }
  }
}