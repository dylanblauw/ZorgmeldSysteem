using System.ComponentModel.DataAnnotations;

namespace ZorgmeldSysteem.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig email adres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [MinLength(6, ErrorMessage = "Wachtwoord moet minimaal 6 karakters zijn")]
        public string Password { get; set; } = string.Empty;
    }
}