namespace Flyingdarts.Meetings.Service.Configuration;

public class DyteApiOptions
{
    public const string SectionName = "DyteApi";
    
    public string BaseUrl { get; set; } = "https://api.dyte.io/v2";
    public string OrganizationId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultPresetName { get; set; } = "default";
} 