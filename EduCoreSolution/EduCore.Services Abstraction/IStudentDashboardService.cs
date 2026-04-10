using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IStudentDashboardService
    {
        Task<Result<StudentDashboardDto>> GetDashboardAsync(string studentId);
    }
}
