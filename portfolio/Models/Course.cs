using System.ComponentModel.DataAnnotations;

namespace portfolio.Models;

public class Course
{
    public long CourseId { get; set; }

    [Required(ErrorMessage = "Course code is required.")]
    [StringLength(15, ErrorMessage = "Code cannot exceed 15 characters.")]
    [RegularExpression(@"^[A-Z]{2,6}[0-9]{1,4}$", ErrorMessage = "Use uppercase letters followed by numbers (e.g., DBMWEB1).")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "Course title is required.")]
    [StringLength(120, ErrorMessage = "Title cannot exceed 120 characters.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Term is required.")]
    [StringLength(30, ErrorMessage = "Term cannot exceed 30 characters.")]
    public string Term { get; set; } = "";

    public string? Description { get; set; }
}