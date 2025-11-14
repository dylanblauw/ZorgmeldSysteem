using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        // ===== AUTH FIELDS =====
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        // ===== BASIC INFO =====
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // ===== AUTHORIZATION =====
        [Required]
        public UserLevel UserLevel { get; set; } = UserLevel.Reporter; // Default

        // ===== MECHANIC-SPECIFIC FIELDS (only for UserLevel = Mechanic) =====
        public MechanicType? MechanicType { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // ===== AUDIT FIELDS =====
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime? LastLogin { get; set; }

        // ===== NAVIGATION PROPERTIES =====
        public ICollection<UserCompany> UserCompanies { get; set; } = new List<UserCompany>();
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

        // ===== COMPUTED PROPERTIES =====
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public string Name => FullName; // Alias voor backward compatibility

        [NotMapped]
        public string Phonenumber => PhoneNumber ?? string.Empty; // Alias

        [NotMapped]
        public MechanicType Type => MechanicType ?? Enums.MechanicType.InternalGeneral; // Alias

        [NotMapped]
        public string UserLevelName => UserLevel switch
        {
            UserLevel.Admin => "Fixility Admin",
            UserLevel.Manager => "Bedrijfsbeheerder",
            UserLevel.Mechanic => "Monteur",
            UserLevel.Reporter => "Melder",
            _ => "Onbekend"
        };

        [NotMapped]
        public bool IsMechanic => UserLevel == UserLevel.Mechanic;

        [NotMapped]
        public bool IsManager => UserLevel == UserLevel.Manager;

        [NotMapped]
        public bool IsAdmin => UserLevel == UserLevel.Admin;
    }
}