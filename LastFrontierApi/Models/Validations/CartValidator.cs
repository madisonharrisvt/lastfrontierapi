using FluentValidation;

namespace LastFrontierApi.Models.Validations
{
  public class CartValidator : AbstractValidator<Cart>
  {
    public CartValidator()
    {
      RuleFor(vm => vm.CartItems).SetCollectionValidator(new CartItemValidator());
    }
  }
}