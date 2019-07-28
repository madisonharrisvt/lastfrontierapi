using System.Security.Claims;
using System.Threading.Tasks;
using LastFrontierApi.Auth;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LastFrontierApi.Controllers
{
  [Route("api/[controller]")]
  public class AuthController : Controller
  {
    private readonly IJwtFactory _jwtFactory;
    private readonly JwtIssuerOptions _jwtOptions;
    private readonly UserManager<AppUser> _userManager;

    public AuthController(UserManager<AppUser> userManager, IJwtFactory jwtFactory,
      IOptions<JwtIssuerOptions> jwtOptions)
    {
      _userManager = userManager;
      _jwtFactory = jwtFactory;
      _jwtOptions = jwtOptions.Value;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Post([FromBody] Credentials credentials)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
      if (identity == null)
        return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));

      // Resolve the user via their email
      var user = await _userManager.FindByEmailAsync(credentials.UserName);
      // Get the roles for the user
      var roles = await _userManager.GetRolesAsync(user);

      var jwt = await Tokens.GenerateJwt(identity, roles, _jwtFactory, credentials.UserName, _jwtOptions,
        new JsonSerializerSettings {Formatting = Formatting.Indented});
      return new OkObjectResult(jwt);
    }

    private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
    {
      if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        return await Task.FromResult<ClaimsIdentity>(null);

      // get the user to verifty
      var userToVerify = await _userManager.FindByNameAsync(userName);

      if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

      // check the credentials
      if (await _userManager.CheckPasswordAsync(userToVerify, password))
        return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));

      // Credentials are invalid, or account doesn't exist
      return await Task.FromResult<ClaimsIdentity>(null);
    }
  }
}