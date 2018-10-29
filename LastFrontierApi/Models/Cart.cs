using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using LastFrontierApi.Models.Validations;

namespace LastFrontierApi.Models
{
    [Validator(typeof(CartValidator))]
    public class Cart
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int PlayerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid Key { get; set; }
        public bool Paid { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
