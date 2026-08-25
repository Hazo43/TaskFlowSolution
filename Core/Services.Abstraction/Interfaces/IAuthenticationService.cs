
using Shared.DTOs.Auth;

namespace Services.Abstraction.Interfaces
{
    public interface IAuthenticationService
    {
        // Login 
        Task<UserResultDto> Login (LoginRequestDto loginReDto);

        // Register
        Task<UserResultDto> Register(RegisterRequestDto registerDto);
    }
}
