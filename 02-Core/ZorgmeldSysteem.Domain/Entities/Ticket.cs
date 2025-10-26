using System.ComponentModel.DataAnnotations;
using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Domain.Entities
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }

        [Required]
        [MaxLength(20)]
        public string TicketCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Note { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public Priority Priority { get; set; } = Priority.Normal;

        public ReactionTime ReactionTime { get; set; } = ReactionTime.CanWait;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? LeadTime { get; set; }

        public DateTime? ScheduledOn { get; set; }

        public DateTime? SolvedOn { get; set; }

        [MaxLength(1000)]
        public string Solution { get; set; } = string.Empty;

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ChangedOn { get; set; }
        public string ChangedBy { get; set; } = string.Empty;

        // Foreign Keys
        public int CompanyID { get; set; }
        public int? MechanicID { get; set; }
        public int? ObjectId { get; set; }

        // Navigation properties
        public Company Company { get; set; }
        public Mechanic? Mechanic { get; set; }
        public Objects? Object { get; set; }
        public bool IsUrgent { get; set; } = false;

    }
}
