using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Auth
{
    public record RegisterRequestDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string DisplayName { get; set; } = null!;
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [RegularExpression(
            @"^(?=.{6,10}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&*()_+{}|:""<>?])[A-Za-z\d!@#$%&*()_+{}|:""<>?]+$",
            ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric character and be between 6 and 10 characters."
         )]
        public string Password { get; set; } = null!;
        [Phone]
        public string? PhoneNumber { get; set; } = null!;

    }
}
