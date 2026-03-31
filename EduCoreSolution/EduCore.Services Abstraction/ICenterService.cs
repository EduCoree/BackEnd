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
    }
}
