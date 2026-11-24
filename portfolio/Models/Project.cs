using System.ComponentModel.DataAnnotations;

namespace portfolio.Models;

public class Project
{
    public long ProjectId { get; set; }

    [Required(ErrorMessage = "Please select a course.")]
    public long? CourseId { get; set; }

    public string CourseCode { get; set; } = "";
    public string CourseTitle { get; set; } = "";

    [Required(ErrorMessage = "Project title is required.")]
    [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Project summary is required.")]
    public string Summary { get; set; } = "";

    [StringLength(255)]
    [RegularExpression(@"^(https?://(www\.)?(youtube\.com/(watch\?v=|shorts/)|youtu\.be/)[A-Za-z0-9_-]{11}.*)?$",
        ErrorMessage = "Enter a valid YouTube URL (e.g., https://youtu.be/xxxxxxxxxxx).")]
    public string? VideoUrl { get; set; }

    [StringLength(255)]
    [RegularExpression(@"^(https?://.+)?$", ErrorMessage = "Repository URL must start with http:// or https://.")]
    public string? RepoUrl { get; set; }

    [Range(2020, 2030, ErrorMessage = "Year must be between 2020 and 2030.")]
    public int YearCreated { get; set; } = 2026;

    public string? ImageUrlsText { get; set; }

    public List<ProjectImage> Images { get; set; } = new();
}