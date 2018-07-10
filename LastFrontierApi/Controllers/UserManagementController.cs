using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserManagementController(UserManager<AppUser> userManager, IMapper mapper,
            ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Registration model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<AppUser>(model);

            var userResult = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!userResult.Succeeded) { return new BadRequestObjectResult(Errors.AddErrorsToModelState(userResult, ModelState)); }

            await _userManager.AddToRoleAsync(userIdentity, "User");

            await _appDbContext.tblStaff.AddAsync(new Staff { IdentityId = userIdentity.Id, Location = model.Location });
            await _appDbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }
    }
}