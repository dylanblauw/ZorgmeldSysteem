using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Application.DTOs.Ticket;

// Dit is wat de API naar de client stuurt (zonder onnodige data)
public class TicketDto
{
    public int TicketID { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public Priority Priority { get; set; }
    public ReactionTime ReactionTime { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LeadTime { get; set; }
    public DateTime? ScheduledOn { get; set; }
    public DateTime? SolvedOn { get; set; }
    public string Solution { get; set; } = string.Empty;

    // Related entities - alleen de info die we nodig hebben
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int? MechanicID { get; set; }
    public string? MechanicName { get; set; }
    public int? ObjectId { get; set; }
    public string? ObjectName { get; set; }
    public bool IsUrgent { get; set; }
}