using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TeamsSetPresence.Core.Interfaces;

namespace TeamsSetPresence.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService teamsService;

        public TeamsController(ITeamsService teamsService)
        {
            this.teamsService = teamsService;
        }


        [HttpGet("GetAccessToken")]
        public async Task<IActionResult> GetAccessToken()
        {
            var token = await teamsService.GetAccessToken();

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var token = await teamsService.GetAccessToken();

            var users = await teamsService.GetUsers(token);
            if (users == null)
            {
                return Unauthorized();
            }

            return Ok(users);
        }

        [HttpPut("UpdateStatus/{userId}")]
        public async Task<IActionResult> UpdateStatus(string userId, string availability, string activity)
        {
            var token = await teamsService.GetAccessToken();
            try
            {
                var response = await teamsService.ChangeStatus(token, userId, availability, activity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Accepted();
        }
    }
}
