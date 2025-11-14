using System.ComponentModel.DataAnnotations;
using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Domain.Entities
{
    public class Mechanic
    {
        [Key]
        public int MechanicID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phonenumber { get; set; } = string.Empty;

        public MechanicType Type { get; set; }

        public bool IsActive { get; set; } = true;

        // Audit fields
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ChangedOn { get; set; }
        public string ChangedBy { get; set; } = string.Empty;

        // Foreign Key
        public int? CompanyID { get; set; }

        // Navigation properties
        public Company? Company { get; set; }
    }
}