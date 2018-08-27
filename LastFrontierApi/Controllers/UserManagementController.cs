using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private static readonly Random random = new Random();

        public UserManagementController(UserManager<AppUser> userManager, IMapper mapper,
            ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var players = _appDbContext.tblPlayer.Include(p => p.Identity).ToList();

            return Ok(players);
        }

        [HttpPut]
        public async Task<IActionResult> CreatePlayer([FromBody] JObject emailObj)
        {
            var temporaryPassword = RandomString(16);

            var newUser = new Registration()
            {
                FirstName = null,
                LastName = null,
                Email = emailObj["email"].ToString(),
                Password = temporaryPassword
            };

            var userIdentity = _mapper.Map<AppUser>(newUser);

            var playerResult = await _userManager.CreateAsync(userIdentity, temporaryPassword);

            if (!playerResult.Succeeded) { return new BadRequestObjectResult(Errors.AddErrorsToModelState(playerResult, ModelState)); }

            await _userManager.AddToRoleAsync(userIdentity, "User");

            await _appDbContext.tblPlayer.AddAsync(new Player { IdentityId = userIdentity.Id });
            await _appDbContext.SaveChangesAsync();

            return new OkObjectResult(userIdentity.Id);

        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()_+?=-0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}