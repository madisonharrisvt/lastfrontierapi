using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Route("api/[controller]")]
    public class ForgotPasswordController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ForgotPasswordController(UserManager<AppUser> userManager, IMapper mapper,
            ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        [HttpPut]
        public async Task<IActionResult> ResetPassword([FromBody] JObject resetObj)
        {
            var email = resetObj["email"].ToString();
            var user = await _userManager.FindByEmailAsync(email);

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            Email.SendPasswordResetLink(email, passwordResetToken);

            return Ok();
        }
    }
}