using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Application.DTOs.Object;

public class ObjectDto
{
    public int ObjectID { get; set; }
    public string ObjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public Priority DefaultPriority { get; set; }
    public ReactionTime DefaultReactionTime { get; set; }

}