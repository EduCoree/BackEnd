using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Content;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/pdf")]
    [Authorize(Roles = "Student")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfLessonService _service;

        public PdfController(IPdfLessonService service)
        {
            _service = service;
        }

        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("{lessonId:int}/signed-url")]
        public async Task<IActionResult> GetSignedUrl(
            int lessonId, CancellationToken ct)
        {
            var result = await _service.GetSignedUrlAsync(lessonId, StudentId, ct);
            return Ok(ApiResponse<SignedUrlResponse>.SuccessResult(result, "Signed URL generated."));
        }
    }
}
