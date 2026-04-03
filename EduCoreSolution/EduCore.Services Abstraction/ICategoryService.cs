using EduCore.Shared.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
   public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesByCenterAsync(int centerId);
        Task<CategoryDto> CreateCategoryAsync(int centerId, CreateCategoryDto dto);
        Task<CategoryDto?> UpdateCategoryAsync(int centerId, int categoryId, UpdateCategoryDto dto);
        Task<(bool Success, bool HasCourses)> DeleteCategoryAsync(int centerId, int categoryId);
    }
}
