using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class UserDetailController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserDetailController(UserManager<AppUser> userManager, IMapper mapper,
            ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return Ok(user);
        }

        //[HttpGet]
        //public IActionResult Get(string userId)
        //{
        //    var result = userId;

        //    return Ok(result);
        //}
    }
}