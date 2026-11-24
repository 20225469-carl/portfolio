using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace portfolio.Controllers;

public class AccountController : Controller
{
    private readonly IConfiguration _config;
    public AccountController(IConfiguration config) => _config = config;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string password)
    {
        var expected = _config["AdminPassword"];
        if (!string.IsNullOrEmpty(expected) && password == expected)
        {
            var claims = new List<Claim> { new(ClaimTypes.Name, "Editor") };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return RedirectToAction("Index", "EditorCourses");
        }

        ViewBag.Error = "Invalid editor password.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Viewer");
    }
}