using System.ComponentModel.DataAnnotations;

namespace ZorgmeldSysteem.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig email adres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minimaal 8 karakters zijn")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Voornaam is verplicht")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Achternaam is verplicht")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gebruikersniveau is verplicht")]
        [Range(2, 4, ErrorMessage = "Gebruikersniveau moet tussen 2 en 4 zijn")]
        public int UserLevel { get; set; }

        [Required(ErrorMessage = "Bedrijf is verplicht")]
        public int CompanyID { get; set; }
    }
}