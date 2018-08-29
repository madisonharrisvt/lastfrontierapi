using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class CheckInController : Controller
    {
        private readonly IPlayerService _playerService;

        public CheckInController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPut]
        public async Task<IActionResult> CreatePlayer([FromBody] JObject emailObj)
        {
            var email = emailObj["email"].ToString();

            try
            {
                var player = await _playerService.CreatePlayerFromEmail(email);
                return new OkObjectResult(player.Id);

            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(emailObj);
            }
        }
    }
}