using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Controllers;

[Authorize]
public class EditorCoursesController : Controller
{
    private readonly CourseRepository _courses;
    public EditorCoursesController(CourseRepository courses) => _courses = courses;

    public async Task<IActionResult> Index() => View(await _courses.GetAllAsync());

    public IActionResult Create() => View(new Course());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid) return View(course);

        try
        {
            await _courses.AddAsync(course);
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Code", "A course with this code already exists.");
            return View(course);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.CheckViolation)
        {
            ModelState.AddModelError("Code", "Course code violated database check rules.");
            return View(course);
        }
    }

    public async Task<IActionResult> Edit(long id)
    {
        var course = await _courses.GetByIdAsync(id);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Course course)
    {
        if (!ModelState.IsValid) return View(course);

        try
        {
            await _courses.UpdateAsync(course);
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Code", "Another course already uses this code.");
            return View(course);
        }
    }

    public async Task<IActionResult> Delete(long id)
    {
        var course = await _courses.GetByIdAsync(id);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            await _courses.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            var course = await _courses.GetByIdAsync(id);
            if (course == null) return NotFound();
            ViewBag.Error = "Cannot delete this course because it still has projects assigned to it.";
            return View("Delete", course);
        }
    }
}