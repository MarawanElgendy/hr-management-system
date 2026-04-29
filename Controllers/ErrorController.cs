namespace HRMS.Controllers;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    public IActionResult Index()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        // default message (for unexpected crashes)
        string message = "An unexpected error occurred.";

        // if it's one of *your* exceptions, show its message
        if (exception is Exception)
        {
            message = exception.Message;
        }

        ViewBag.ErrorMessage = message;
        return View();
    }
}
