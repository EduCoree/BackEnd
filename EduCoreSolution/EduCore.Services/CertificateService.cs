using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Progress;
using EduCore.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;

        public CertificateService(IUnitOfWork uow, UserManager<User> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<List<CertificateResponse>> GetMyCertificatesAsync(
            string studentId, CancellationToken ct = default)
        {
            var certRepo = _uow.GetRepository<Certificate, int>();
            var all = await certRepo.GetAllAsync();
            var myCerts = all
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.IssuedAt)
                .ToList();

            var courseRepo = _uow.GetRepository<Course, int>();
            var student = await _userManager.FindByIdAsync(studentId);
            var studentName = student?.Name ?? studentId;

            var result = new List<CertificateResponse>();
            foreach (var cert in myCerts)
            {
                var course = await courseRepo.GetByIdAsync(cert.CourseId);
                var courseTitle = course?.Title ?? cert.CourseId.ToString();
                var uuid = ExtractUuid(cert.CertificateUrl);

                result.Add(new CertificateResponse
                {
                    Id = cert.Id,
                    CourseId = cert.CourseId,
                    CourseTitle = courseTitle,
                    StudentName = studentName,
                    IssuedAt = cert.IssuedAt,
                    CertificateUuid = uuid,
                    CertificateUrl = cert.CertificateUrl ?? string.Empty
                });
            }

            return result;
        }

        public async Task<CertificateResponse> GetCertificateAsync(
            string certificateUuid, CancellationToken ct = default)
        {
            var certRepo = _uow.GetRepository<Certificate, int>();
            var all = await certRepo.GetAllAsync();
            var cert = all.FirstOrDefault(c =>
                c.CertificateUrl != null && c.CertificateUrl.EndsWith(certificateUuid));

            if (cert is null)
                throw new NotFoundException("Certificate not found.");

            var courseRepo = _uow.GetRepository<Course, int>();
            var course = await courseRepo.GetByIdAsync(cert.CourseId);
            var courseTitle = course?.Title ?? cert.CourseId.ToString();

            var student = await _userManager.FindByIdAsync(cert.StudentId);
            var studentName = student?.Name ?? cert.StudentId;

            return new CertificateResponse
            {
                Id = cert.Id,
                CourseId = cert.CourseId,
                CourseTitle = courseTitle,
                StudentName = studentName,
                IssuedAt = cert.IssuedAt,
                CertificateUuid = certificateUuid,
                CertificateUrl = cert.CertificateUrl ?? string.Empty
            };
        }

        public async Task<string> GetCertificateHtmlAsync(
            string certificateUuid, CancellationToken ct = default)
        {
            var cert = await GetCertificateAsync(certificateUuid, ct);
            var linkedInUrl = Uri.EscapeDataString(cert.CertificateUrl);

            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  <title>Certificate of Completion</title>
  <style>
    * {{ margin: 0; padding: 0; box-sizing: border-box; }}
    body {{
      font-family: 'Georgia', serif;
      background: #f9fafb;
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 2rem;
    }}
    .certificate {{
      background: white;
      border: 6px solid #0d9488;
      border-radius: 1.5rem;
      padding: 4rem;
      max-width: 800px;
      width: 100%;
      text-align: center;
      box-shadow: 0 20px 60px rgba(0,0,0,0.1);
    }}
    .logo {{
      font-size: 1.5rem;
      font-weight: 900;
      color: #0d9488;
      letter-spacing: 0.1em;
      margin-bottom: 0.5rem;
    }}
    .subtitle {{
      font-size: 0.75rem;
      text-transform: uppercase;
      letter-spacing: 0.2em;
      color: #6b7280;
      margin-bottom: 2.5rem;
    }}
    h1 {{
      font-size: 2rem;
      color: #111827;
      margin-bottom: 1.5rem;
    }}
    .certifies {{
      font-size: 1rem;
      color: #6b7280;
      margin-bottom: 0.75rem;
    }}
    .student-name {{
      font-size: 2.5rem;
      font-weight: bold;
      color: #0d9488;
      margin-bottom: 1rem;
      border-bottom: 2px solid #e5e7eb;
      padding-bottom: 1rem;
    }}
    .completed-text {{
      font-size: 1rem;
      color: #6b7280;
      margin-bottom: 0.75rem;
    }}
    .course-title {{
      font-size: 1.5rem;
      font-weight: bold;
      color: #111827;
      margin-bottom: 2rem;
    }}
    .issued {{
      font-size: 0.875rem;
      color: #9ca3af;
      margin-bottom: 2rem;
    }}
    .uuid {{
      font-size: 0.7rem;
      color: #d1d5db;
      font-family: monospace;
      margin-bottom: 2rem;
    }}
    .actions {{
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap;
    }}
    .btn {{
      padding: 0.75rem 2rem;
      border-radius: 9999px;
      font-weight: bold;
      font-size: 0.875rem;
      text-decoration: none;
      cursor: pointer;
      border: none;
    }}
    .btn-primary {{
      background: #0d9488;
      color: white;
    }}
    .btn-secondary {{
      background: white;
      color: #0d9488;
      border: 2px solid #0d9488;
    }}
    @media print {{
      body {{ background: white; padding: 0; }}
      .actions {{ display: none; }}
      .certificate {{ box-shadow: none; border-color: #0d9488; }}
    }}
  </style>
</head>
<body>
  <div class=""certificate"">
    <div class=""logo"">EduCore</div>
    <div class=""subtitle"">Certificate of Completion</div>
    <h1>Certificate of Completion</h1>
    <p class=""certifies"">This certifies that</p>
    <div class=""student-name"">{cert.StudentName}</div>
    <p class=""completed-text"">has successfully completed</p>
    <div class=""course-title"">{cert.CourseTitle}</div>
    <div class=""issued"">Issued on {cert.IssuedAt:MMMM dd, yyyy}</div>
    <div class=""uuid"">Certificate ID: {cert.CertificateUuid}</div>
    <div class=""actions"">
      <button class=""btn btn-primary"" onclick=""window.print()"">
        Print / Save as PDF
      </button>
      <a class=""btn btn-secondary""
         href=""https://www.linkedin.com/sharing/share-offsite/?url={linkedInUrl}""
         target=""_blank"">
        Share on LinkedIn
      </a>
    </div>
  </div>
</body>
</html>";
        }

        // ── Helpers ──────────────────────────────────────

        private static string ExtractUuid(string? certificateUrl)
        {
            if (string.IsNullOrEmpty(certificateUrl))
                return string.Empty;

            var lastSlash = certificateUrl.LastIndexOf('/');
            return lastSlash >= 0 && lastSlash < certificateUrl.Length - 1
                ? certificateUrl.Substring(lastSlash + 1)
                : certificateUrl;
        }
    }
}
