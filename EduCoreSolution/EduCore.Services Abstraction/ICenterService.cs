using EduCore.Shared.DTOs.Centers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface ICenterService
    {
        Task<IEnumerable<CenterDto>> GetAllCentersAsync();
        Task<CenterDto?> GetCenterByIdAsync(int id);
        Task<CenterDto> CreateCenterAsync(CreateCenterDto dto);
        Task<CenterDto?> UpdateCenterAsync(int id, UpdateCenterDto dto);
        Task<bool> DeleteCenterAsync(int id);

        Task<CenterDto?> UpdateSocialLinksAsync(int id, SocialLinksDto dto);
        Task<CenterDto?> UpdateLogoAsync(int id, string logoUrl);

        Task<CenterDto?> UpdateSettingsAsync(int id, CenterSettingsDto dto, string userId);
    }
}
