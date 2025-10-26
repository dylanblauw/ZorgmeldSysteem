using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Application.DTOs.Ticket;

// Dit wordt gebruikt om een NIEUWE ticket te maken
// Bevat alleen de velden die de gebruiker kan invullen
public class CreateTicketDto
{
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? Location { get; set; }
    public Priority Priority { get; set; } = Priority.Normal;
    public ReactionTime ReactionTime { get; set; } = ReactionTime.CanWait;
    public int CompanyID { get; set; }
    public int? MechanicID { get; set; }
    public int? ObjectId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsUrgent { get; set; } = false;

    // TicketCode, CreatedOn, Status worden automatisch gegenereerd
}