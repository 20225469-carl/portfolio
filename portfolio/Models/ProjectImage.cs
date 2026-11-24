namespace portfolio.Models;

public class ProjectImage
{
    public long ImageId { get; set; }
    public long ProjectId { get; set; }
    public string ImageUrl { get; set; } = "";
}