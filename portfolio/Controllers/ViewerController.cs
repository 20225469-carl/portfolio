using Microsoft.AspNetCore.Mvc;
using portfolio.Data;

namespace portfolio.Controllers;

public class ViewerController : Controller
{
    private readonly CourseRepository _courses;
    private readonly ProjectRepository _projects;
    private readonly ProjectImageRepository _images;

    public ViewerController(CourseRepository courses, ProjectRepository projects, ProjectImageRepository images)
    {
        _courses = courses;
        _projects = projects;
        _images = images;
    }

    public async Task<IActionResult> Index(long? courseId)
    {
        ViewBag.Courses = await _courses.GetAllAsync();
        ViewBag.SelectedCourseId = courseId;
        var projects = await _projects.GetAllAsync(courseId);
        return View(projects);
    }

    public async Task<IActionResult> Project(long id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project == null) return NotFound();

        project.Images = await _images.GetByProjectIdAsync(id);
        return View(project);
    }
}