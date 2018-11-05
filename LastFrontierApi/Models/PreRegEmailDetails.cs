using System;
using System.Collections.Generic;
using Stripe;

namespace LastFrontierApi.Models
{
    public class PreRegEmailDetails
    {
        public string Email { get; set; }
        public List<PreRegCharacterDetail> PreRegCharacterDetails { get; set; }
        public Guid OrderID { get; set; }
        public StripeCard BillingDetails { get; set; }
        public int GrandTotal { get; set; }
        public Event Event { get; set; }
    }
}
