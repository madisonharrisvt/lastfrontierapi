using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
  [Route("api/[controller]")]
  public class AccountsController : Controller
  {
    private readonly ApplicationDbContext _appDbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public AccountsController(UserManager<AppUser> userManager, IMapper mapper, ApplicationDbContext appDbContext,
      RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _mapper = mapper;
      _appDbContext = appDbContext;
    }

    // POST api/accounts
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Registration model)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var userIdentity = _mapper.Map<AppUser>(model);

      var userResult = await _userManager.CreateAsync(userIdentity, model.Password);

      if (!userResult.Succeeded)
        return new BadRequestObjectResult(Errors.AddErrorsToModelState(userResult, ModelState));

      await _userManager.AddToRoleAsync(userIdentity, "User");

      await _appDbContext.tblPlayer.AddAsync(new Player {IdentityId = userIdentity.Id});
      await _appDbContext.SaveChangesAsync();

      return new OkObjectResult("Account created");
    }
  }
}