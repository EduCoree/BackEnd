using Microsoft.AspNetCore.Mvc;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected string CurrentLang =>
            (Request.Headers["Accept-Language"]
                .FirstOrDefault() ?? "en")
                .Split(',')[0].Trim().ToLower()
                .StartsWith("ar") ? "ar" : "en";
    }
}