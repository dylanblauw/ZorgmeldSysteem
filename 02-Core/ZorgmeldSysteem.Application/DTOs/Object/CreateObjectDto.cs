using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Application.DTOs.Object;

public class CreateObjectDto
{
    public string ObjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? InstallationDate { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public int CompanyID { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public Priority DefaultPriority { get; set; } = Priority.Normal;
    public ReactionTime DefaultReactionTime { get; set; } = ReactionTime.Within24Hours;
}