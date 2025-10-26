using System.ComponentModel.DataAnnotations;
using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Domain.Entities
{
    public class Objects
    {
        [Key]
        public int ObjectID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ObjectCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [MaxLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        public DateTime? InstallationDate { get; set; }

        public DateTime? LastMaintenance { get; set; }

        public DateTime? NextMaintenance { get; set; }

        // Audit fields
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ChangedOn { get; set; }
        public string ChangedBy { get; set; } = string.Empty;

        // Foreign Key
        public int CompanyID { get; set; }

        // Navigation properties
        public Company Company { get; set; }
        public List<Ticket> Tickets { get; set; } = new();
        public Priority DefaultPriority { get; set; } = Priority.Normal;
        public ReactionTime DefaultReactionTime { get; set; } = ReactionTime.Within24Hours;

    }
}