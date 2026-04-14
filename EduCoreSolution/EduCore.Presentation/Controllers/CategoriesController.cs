using EduCore.Services;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Categories;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/centers/{centerId:int}/[controller]")]
    public class CategoriesController : BaseController 
    {
        private readonly ICategoryService _categoryService;
        private readonly TranslationService _tr; 

        public CategoriesController(ICategoryService categoryService, TranslationService tr)
        {
            _categoryService = categoryService;
            _tr = tr; 
        }

        // GET api/centers/5/categories
        [HttpGet]
        public async Task<IActionResult> GetAll(int centerId)
        {
            var categories = await _categoryService.GetCategoriesByCenterAsync(centerId);

          
            foreach (var dto in categories)
                await _tr.TranslateAsync(dto, "Category", dto.Id, CurrentLang,
                    ("Name", (d, v) => d.Name = v));

            return Ok(categories);
        }

        // POST api/centers/5/categories
        [HttpPost]
        public async Task<IActionResult> Create(int centerId, [FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _categoryService.CreateCategoryAsync(centerId, dto);
            return CreatedAtAction(nameof(GetAll), new { centerId }, created);
        }

        // PUT api/centers/5/categories/3
        [HttpPut("{categoryId:int}")]
        public async Task<IActionResult> Update(int centerId, int categoryId, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _categoryService.UpdateCategoryAsync(centerId, categoryId, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        // DELETE api/centers/5/categories/3
        [HttpDelete("{categoryId:int}")]
        public async Task<IActionResult> Delete(int centerId, int categoryId)
        {
           
            await _tr.DeleteAllAsync("Category", categoryId);

            var (success, hasCourses) = await _categoryService.DeleteCategoryAsync(centerId, categoryId);
            if (hasCourses) return Conflict("Cannot delete category with existing courses.");
            if (!success) return NotFound();
            return NoContent();
        }
    }
}