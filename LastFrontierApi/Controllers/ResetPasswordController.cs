using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

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

      if (!result.Succeeded)
      {
        var errorList = new Dictionary<string, string>();
        foreach (var error in result.Errors) errorList.Add(error.Code, error.Description);
        return BadRequest(errorList);
      }

      return Ok();
    }
  }
}