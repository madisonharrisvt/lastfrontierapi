﻿using System;
using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Stripe;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin, User")]
    [Route("api/[controller]")]
    public class CheckOutController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _appDbContext;
        private readonly LfContext _lfContext;

        private const string StripeKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"; // todo: get production key when publishing

        public CheckOutController(UserManager<AppUser> userManager, ApplicationDbContext appDbContext, LfContext lfContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _lfContext = lfContext;
        }
        
        [HttpPost]
        public IActionResult CheckOut([FromBody] JObject token)
        {
            StripeConfiguration.SetApiKey(StripeKey);
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

                var currentEvent = _lfContext.tblEvent.FirstOrDefault(e => e.IsActiveEvent);

                if (currentEvent == null) return BadRequest("There is no current event!");

                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem == cart.CartItems.First())
                    {
                        amount += 40;
                    }
                    else
                    {
                        amount += 10;
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

                //if (charge.Outcome.StripeResponse.)

                cart.Paid = true;

                _lfContext.tblCart.Update(cart);

                player.VolunteerPoints = player.VolunteerPoints - totalVp;

                _appDbContext.tblPlayer.Update(player);
                _appDbContext.SaveChanges();

                // Event 1-6 = 40XP
                // Event 7-12 = 20XP
                // Event 13+ = 10XP
                var characterDetailForEmail = new List<PreRegCharacterDetail>();
                foreach (var cartItem in cart.CartItems)
                {
                    var characterTotalXp = 0;
                    characterTotalXp += cartItem.PurchaseXp;
                    characterTotalXp += cartItem.VpToXp;

                    var character = _lfContext.tblCharacter.FirstOrDefault(c => c.Id == cartItem.CharacterId);

                    if (character == null) return BadRequest("Unable to get character data for characterid " + cartItem.CharacterId); 

                    if (character.AccumulatedXP == null) character.AccumulatedXP = 0;
                    if (character.AvailableXP == null) character.AvailableXP = 0;

                    var characterEvents = _lfContext.tblCharacterEvents.Where(e => e.CharacterId == character.Id);

                    int baseXp;
                    var totalEvents = characterEvents.Count();
                    if (totalEvents < 6)
                    {
                        baseXp = 40;
                    }
                    else if (totalEvents < 12)
                    {
                        baseXp = 20;
                    }
                    else baseXp = 10;

                    characterTotalXp += baseXp;

                    character.AccumulatedXP += characterTotalXp;
                    character.AvailableXP += characterTotalXp;

                    var newCharacterEvent = new CharacterEvent
                    {
                        VpToXp = cartItem.VpToXp,
                        XpBought = cartItem.PurchaseXp,
                        EventId = currentEvent.Id,
                        CharacterId = cartItem.CharacterId
                    };

                    _lfContext.tblCharacter.Update(character);
                    _lfContext.tblCharacterEvents.Add(newCharacterEvent);

                    var characterToAdd = new PreRegCharacterDetail
                    {
                        Character = character,
                        CartItem = cartItem,
                        BaseXp = baseXp,
                        BaseXpCost = characterDetailForEmail.FirstOrDefault() == null ? 40 : 10
                    };

                    characterDetailForEmail.Add(characterToAdd);
                }

                _lfContext.SaveChanges();

                var stripeResonse = JObject.Parse(charge.StripeResponse.ObjectJson);

                var billingDetails = charge.Source.Card;
                var email = token["email"].ToString();

                var emailDetails = new PreRegEmailDetails
                {
                    Email = email,
                    PreRegCharacterDetails = characterDetailForEmail,
                    OrderID = cart.Key,
                    BillingDetails = billingDetails,
                    GrandTotal = amount,
                    Event = currentEvent
                };


                Email.SendPreRegConfirmationEmail(emailDetails);

                return Ok(charge);
            }
            catch (Exception e)
            {
                return BadRequest("An unknown error has ocurred");
            }
        }
    }
}