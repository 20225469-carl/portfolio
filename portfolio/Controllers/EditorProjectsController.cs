using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Controllers;

[Authorize]
public class EditorProjectsController : Controller
{
    private readonly ProjectRepository _projects;
    private readonly CourseRepository _courses;
    private readonly ProjectImageRepository _images;

    public EditorProjectsController(ProjectRepository projects, CourseRepository courses, ProjectImageRepository images)
    {
        _projects = projects;
        _courses = courses;
        _images = images;
    }

    private async Task FillCoursesAsync()
    {
        ViewBag.Courses = new SelectList(await _courses.GetAllAsync(), "CourseId", "Code");
    }

    private List<string> ParseAndValidateImageUrls(string? rawText)
    {
        var urls = new List<string>();
        if (string.IsNullOrWhiteSpace(rawText)) return urls;

        var lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (!Regex.IsMatch(line, @"^https?://[^\s]+$"))
            {
                ModelState.AddModelError("ImageUrlsText", $"Invalid image URL format: {line}");
            }
            urls.Add(line);
        }
        return urls;
    }

    public async Task<IActionResult> Index() => View(await _projects.GetAllAsync());

    public async Task<IActionResult> Create()
    {
        await FillCoursesAsync();
        return View(new Project());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        var imageUrls = ParseAndValidateImageUrls(project.ImageUrlsText);

        if (!ModelState.IsValid)
        {
            await FillCoursesAsync();
            return View(project);
        }

        try
        {
            await _projects.CreateWithImagesAsync(project, imageUrls);
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("", "Transaction rolled back: Duplicate project title or duplicate image URL detected.");
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.CheckViolation)
        {
            ModelState.AddModelError("", "Transaction rolled back: A value failed a database CHECK constraint.");
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            ModelState.AddModelError("CourseId", "Transaction rolled back: Selected course does not exist.");
        }

        await FillCoursesAsync();
        return View(project);
    }

    public async Task<IActionResult> Edit(long id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project == null) return NotFound();

        var existingImages = await _images.GetByProjectIdAsync(id);
        project.ImageUrlsText = string.Join(Environment.NewLine, existingImages.Select(i => i.ImageUrl));

        await FillCoursesAsync();
        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Project project)
    {
        var imageUrls = ParseAndValidateImageUrls(project.ImageUrlsText);

        if (!ModelState.IsValid)
        {
            await FillCoursesAsync();
            return View(project);
        }

        try
        {
            await _projects.UpdateWithImagesAsync(project, imageUrls);
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("", "Transaction rolled back: Duplicate project title or duplicate image URL detected.");
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.CheckViolation)
        {
            ModelState.AddModelError("", "Transaction rolled back: A value failed a database CHECK constraint.");
        }

        await FillCoursesAsync();
        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        await _projects.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}