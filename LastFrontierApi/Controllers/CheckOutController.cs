using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Stripe;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class CheckOutController : ControllerBase
    {
        private const string STRIPE_KEY = "xxxxxxxxxxxxxxxxxxxxx"; // todo: get production key when publishing
        [HttpPost]
        public IActionResult CheckOut([FromBody] JObject token)
        {
            StripeConfiguration.SetApiKey(STRIPE_KEY);

            var options = new StripeChargeCreateOptions
            {
                Amount = 2000,
                Currency = "usd",
                Description = "Test Charge",
                SourceTokenOrExistingSourceId = token["id"].ToString(),
            };
            var service = new StripeChargeService();
            var charge = service.Create(options);


            return Ok(charge);
        }
    }
}