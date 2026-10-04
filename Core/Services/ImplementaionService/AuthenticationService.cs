using Domain.Entites;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Abstraction.Interfaces;
using Shared.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.ImplementaionService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        // Login
        public async Task<UserResultDto> Login(LoginRequestDto _loginReDto)
        {
            var userEmail = await _userManager.FindByEmailAsync(_loginReDto.Email);
            if (userEmail is null)
                throw new UnauthorizedException("Invalid email or passord");

            var checkPassword = await _userManager.CheckPasswordAsync(userEmail, _loginReDto.Password);
            if (!checkPassword)
                throw new UnauthorizedException("Invalid email or passord");

            return new UserResultDto(userEmail.DisplayName, await CreateTokenAsync(userEmail), userEmail.Email);
        }

        // register
        public async Task<UserResultDto> Register(RegisterRequestDto _registerDto)
        {
            var user = new User()
            {
                DisplayName = _registerDto.DisplayName,
                UserName = _registerDto.UserName,
                Email = _registerDto.Email,
                PhoneNumber = _registerDto.PhoneNumber,

            };

            var resultCreateUser = await _userManager.CreateAsync(user, _registerDto.Password);

            if (resultCreateUser.Succeeded == false)
                throw new UnauthorizedException($"Created faild"); ;

            return new UserResultDto(user.DisplayName, await CreateTokenAsync(user), user.Email);

        }

        // Token 
        private async Task<string> CreateTokenAsync(User user)
        {

            // Private Claims 

            var authclaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.DisplayName),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authclaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Key
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ?? string.Empty));

            var token = new JwtSecurityToken
                (
                  audience: _configuration["JWT:Audience"],
                  issuer: _configuration["JWT:Issuer"],
                  expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:ExpirationInDays"] ?? "0")),
                  claims: authclaims,
                  signingCredentials: new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature)

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
