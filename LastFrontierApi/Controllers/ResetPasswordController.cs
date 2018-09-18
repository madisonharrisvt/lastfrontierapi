using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Route("api/[controller]")]
    public class ResetPasswordController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ResetPasswordController(UserManager<AppUser> userManager, IMapper mapper,
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
            var token = resetObj["token"].ToString();
            var password = resetObj["password"].ToString();
            var user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded) { return new BadRequestResult(); }

            return Ok();
        }
    }
}