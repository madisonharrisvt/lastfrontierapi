using System;
using System.Linq;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Stripe;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class CheckOutController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _appDbContext;
        private readonly LfContext _lfContext;

        public CheckOutController(UserManager<AppUser> userManager, ApplicationDbContext appDbContext, LfContext lfContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _lfContext = lfContext;
        }
        private const string STRIPE_KEY = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"; // todo: get production key when publishing
        [HttpPost]
        public IActionResult CheckOut([FromBody] JObject token)
        {
            StripeConfiguration.SetApiKey(STRIPE_KEY);
            try
            {
                var amount = 0;
                var totalVp = 0;
                var totalXp = 0;
                var key = Guid.Parse(token["cartKey"].ToString());
                var cart = _lfContext.tblCart.Include(c => c.CartItems).FirstOrDefault(c => c.Key == key);

                if (cart == null) return BadRequest("Unable to retrieve cart data.");

                var player = _appDbContext.tblPlayer.FirstOrDefault(p => p.Id == cart.PlayerId);

                if (player == null) return BadRequest("Unable to retrieve player data.");

                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem == cart.CartItems.First())
                    {
                        amount = amount + 40;
                    }
                    else
                    {
                        amount = amount + 10;
                    }

                    amount += cartItem.PurchaseXp;
                    totalVp += cartItem.VpToXp;
                    totalXp = totalXp + cartItem.PurchaseXp + cartItem.VpToXp;
                }

                var options = new StripeChargeCreateOptions
                {
                    Amount = amount * 100,
                    Currency = "usd",
                    Description = "Test Charge",
                    SourceTokenOrExistingSourceId = token["id"].ToString(),
                };
                var service = new StripeChargeService();
                var charge = service.Create(options);

                cart.Paid = true;

                _lfContext.tblCart.Update(cart);

                player.VolunteerPoints = player.VolunteerPoints - totalVp;

                _appDbContext.tblPlayer.Update(player);
                _appDbContext.SaveChanges();

                foreach (var cartItem in cart.CartItems)
                {
                    var character = _lfContext.tblCharacter.FirstOrDefault(c => c.Id == cartItem.CharacterId);
                    if (character == null) return BadRequest("Unable to get character data.");

                    if (character.AccumulatedXP == null) character.AccumulatedXP = 0;
                    if (character.AvailableXP == null) character.AvailableXP = 0;

                    character.AccumulatedXP += totalXp;
                    character.AvailableXP += totalXp;

                    _lfContext.tblCharacter.Update(character);
                }

                _lfContext.SaveChanges();

                return Ok(charge);
            }
            catch (Exception e)
            {
                return BadRequest("An unknown error has ocurred");
            }
            
        }
    }
}