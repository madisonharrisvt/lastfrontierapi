using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/[controller]")]
    public class PreRegistrationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _appDbContext;
        private readonly LfContext _lfContext;

        public PreRegistrationController(UserManager<AppUser> userManager, ApplicationDbContext appDbContext, LfContext lfContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _lfContext = lfContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoggedInPlayer()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByEmailAsync(userName);

            var player = _appDbContext.tblPlayer.Include(p => p.Identity)
                .FirstOrDefault(p => p.Identity.Equals(user));

            if (player == null) return new BadRequestObjectResult("Unable to find player");

            return Ok(player);
        }

        [HttpPut]
        public async Task<IActionResult> AddToCart([FromBody] Cart cart)
        {
            try
            {
                if (cart == null) return BadRequest("Unable to create cart data");
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByEmailAsync(userName);

                var player = _appDbContext.tblPlayer.Include(p => p.Identity)
                    .FirstOrDefault(p => p.Identity.Equals(user));

                if (player == null) return BadRequest("Unable to find player");

                var activeEvent = _lfContext.tblEvent.FirstOrDefault(e => e.IsActiveEvent);

                if (activeEvent == null) return BadRequest("Unable to retrieve the active event");

                cart.PlayerId = player.Id;
                cart.EventId = activeEvent.Id;
                cart.CreatedDate = System.DateTime.UtcNow;
                cart.Key = Guid.NewGuid();
                cart.Paid = false;

                _lfContext.tblCart.Add(cart);
                _lfContext.tblCartItem.AddRange(cart.CartItems);
                _lfContext.SaveChanges();
                return Ok(cart.Key);
            }
            catch (Exception e)
            {
                return BadRequest("An unknown error occurred.");
            }
        }
    }
}