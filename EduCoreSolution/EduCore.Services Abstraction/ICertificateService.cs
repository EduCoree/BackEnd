using EduCore.Shared.DTOs.Progress;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface ICertificateService
    {
        Task<List<CertificateResponse>> GetMyCertificatesAsync(
            string studentId, CancellationToken ct = default);

        Task<CertificateResponse> GetCertificateAsync(
            string certificateUuid, CancellationToken ct = default);

        Task<string> GetCertificateHtmlAsync(
            string certificateUuid, CancellationToken ct = default);
    }
}
