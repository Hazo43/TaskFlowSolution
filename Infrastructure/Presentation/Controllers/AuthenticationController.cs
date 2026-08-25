using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Interfaces;
using Shared.DTOs.Auth;

namespace Presentation.Controllers
{
    public class AuthenticationController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]  //  Post : BaseUrl/api/Authentication/login
        public async Task<ActionResult<UserResultDto>> Login (LoginRequestDto loginDto)
        {
            var user = await _authenticationService.Login(loginDto);
            return Ok(user);
        }


        [HttpPost("register")] //  Post : BaseUrl/api/Authentication/register
        public async Task<ActionResult<UserResultDto>> Register (RegisterRequestDto registerDto)
        {
            var user = await _authenticationService.Register(registerDto);
            return Ok(user);
        }

    }
}
