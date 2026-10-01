using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}