using FluentValidation;

namespace LastFrontierApi.Models.Validations
{
    public class CartValidator : AbstractValidator<Cart>
    {
        public CartValidator()
        {
            //RuleFor(vm => vm.CartItems).NotEmpty().WithMessage("Must add at least one item to the cart");
            //RuleFor(vm => vm.EventId).GreaterThan(0).WithMessage("lmao this is dumb");
            RuleFor(vm => vm.CartItems).SetCollectionValidator(new CartItemValidator());
        }
    }
}
