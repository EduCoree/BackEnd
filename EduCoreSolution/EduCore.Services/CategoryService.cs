using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesByCenterAsync(int centerId)
        {
            var repo = _unitOfWork.GetRepository<Category, int>();
            var categories = await repo.GetAllAsync();
            var filtered = categories.Where(c => c.CenterId == centerId);
            return _mapper.Map<IEnumerable<CategoryDto>>(filtered);
        }


        public async Task<CategoryDto> CreateCategoryAsync(int centerId, CreateCategoryDto dto)
        {
            var repo = _unitOfWork.GetRepository<Category, int>();

          
            var slug = dto.Name.ToLower()
                               .Trim()
                               .Replace(" ", "-")
                               .Replace("_", "-");

            var category = new Category
            {
                CenterId = centerId,
                Name = dto.Name,
                Slug = slug,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }



        public async Task<CategoryDto?> UpdateCategoryAsync(int centerId, int categoryId, UpdateCategoryDto dto)
        {
            var repo = _unitOfWork.GetRepository<Category, int>();
            var category = await repo.GetByIdAsync(categoryId);

            
            if (category is null || category.CenterId != centerId) return null;

            
            category.Name = dto.Name;
            category.Slug = dto.Name.ToLower()
                                    .Trim()
                                    .Replace(" ", "-")
                                    .Replace("_", "-");

            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }


        public async Task<(bool Success, bool HasCourses)> DeleteCategoryAsync(int centerId, int categoryId)
        {
            var repo = _unitOfWork.GetRepository<Category, int>();
            var category = await repo.GetByIdAsync(categoryId);

            
            if (category is null || category.CenterId != centerId)
                return (false, false);

           
            if (category.Courses.Any())
                return (false, true);

            repo.Remove(category);
            await _unitOfWork.SaveChangesAsync();
            return (true, false);
        }
    }
}
