using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
  [Authorize(Policy = "ApiUser", Roles = "Admin")]
  [Route("api/[controller]")]
  public class RoleController : Controller
  {
    private readonly ApplicationDbContext _appDbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public RoleController(UserManager<AppUser> userManager, IMapper mapper,
      ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _mapper = mapper;
      _appDbContext = appDbContext;
    }

    [HttpPut]
    public async Task<IActionResult> AddUserToRole([FromBody] JObject roleObj)
    {
      var email = roleObj["email"].ToString();
      var role = roleObj["role"].ToString();

      var user = await _userManager.FindByEmailAsync(email);
      var usersRoles = await _userManager.GetRolesAsync(user);

      if (usersRoles.Contains(role)) return new BadRequestObjectResult($"User already has role {role}");

      var addRoleResult = await _userManager.AddToRoleAsync(user, role);

      if (!addRoleResult.Succeeded) return new BadRequestObjectResult(addRoleResult);

      /*var removeRoleResut = await _userManager.RemoveFromRolesAsync(user, usersRoles);

        if (!removeRoleResut.Succeeded)
        {
            return new BadRequestObjectResult($"Role {role} was successfully added, but {removeRoleResut}");
        }
        */

      return Ok();
    }
  }
}