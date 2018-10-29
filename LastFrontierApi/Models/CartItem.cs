using FluentValidation.Attributes;
using LastFrontierApi.Models.Validations;

namespace LastFrontierApi.Models
{
    [Validator(typeof(CartItemValidator))]
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int CharacterId { get; set; }
        public int VpToXp { get; set; }
        public int PurchaseXp { get; set; }

        public Cart Cart { get; set; }
    }
}
