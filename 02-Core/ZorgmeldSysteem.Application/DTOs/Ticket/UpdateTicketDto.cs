using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Application.DTOs.Ticket;

// Voor het UPDATEN van een bestaande ticket
// Alle velden zijn nullable - alleen wat ingevuld is wordt geupdate
public class UpdateTicketDto
{
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
    public TicketStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public ReactionTime? ReactionTime { get; set; }
    public int? MechanicID { get; set; }
    public DateTime? ScheduledOn { get; set; }
    public string? Solution { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public bool IsUrgent { get; set; }
}