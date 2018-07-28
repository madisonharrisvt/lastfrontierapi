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

        ////[HttpPost]
        ////public async Task<IActionResult> UpdateOrCreateUser([FromBody]Registration model)
        ////{
        ////    if (!ModelState.IsValid)
        ////    {
        ////        return BadRequest(ModelState);
        ////    }

        ////    var userIdentity = _mapper.Map<AppUser>(model);

        ////    var user = await _userManager.FindByEmailAsync(model.Email);

        ////    if (user != null)
        ////    {
        ////        //var userResult = await _userManager.CreateAsync(userIdentity, model.Password);

        ////        //if (!userResult.Succeeded) { return new BadRequestObjectResult(Errors.AddErrorsToModelState(userResult, ModelState)); }

        ////        //await _userManager.AddToRoleAsync(userIdentity, "User");

        ////        //await _appDbContext.tblPlayer.AddAsync(new Player { IdentityId = userIdentity.Id });
        ////        //await _appDbContext.SaveChangesAsync();

        ////        //return new OkObjectResult("Account created");

                
        ////    }

            

        }

        //[HttpGet]
        //public IActionResult Get(string userId)
        //{
        //    var result = userId;

        //    return Ok(result);
        //}
    }
}