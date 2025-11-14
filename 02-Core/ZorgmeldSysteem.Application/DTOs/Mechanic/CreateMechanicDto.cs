using System.ComponentModel.DataAnnotations;
using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Application.DTOs.Mechanic
{
    public class CreateMechanicDto
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is verplicht")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phonenumber { get; set; }

        [Required]
        public MechanicType Type { get; set; }

        [Required(ErrorMessage = "Bedrijf is verplicht")]
        public int CompanyID { get; set; }

        // Nieuw: tijdelijk wachtwoord
        [Required(ErrorMessage = "Tijdelijk wachtwoord is verplicht")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minimaal 8 karakters zijn")]
        public string TempPassword { get; set; } = string.Empty;
    }
}