using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Progress;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificateService _service;

        public CertificatesController(ICertificateService service)
        {
            _service = service;
        }

        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCertificates(CancellationToken ct)
        {
            var result = await _service.GetMyCertificatesAsync(StudentId, ct);
            return Ok(ApiResponse<List<CertificateResponse>>.SuccessResult(result));
        }

        [HttpGet("{certificateUuid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCertificate(string certificateUuid, CancellationToken ct)
        {
            var result = await _service.GetCertificateAsync(certificateUuid, ct);
            return Ok(ApiResponse<CertificateResponse>.SuccessResult(result));
        }

        [HttpGet("{certificateUuid}/view")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewCertificate(string certificateUuid, CancellationToken ct)
        {
            var html = await _service.GetCertificateHtmlAsync(certificateUuid, ct);
            return Content(html, "text/html");
        }
    }
}
