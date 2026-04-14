using EduCore.Presentation.Controllers;
using EduCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers
{
    [Route("api/translations")]
    //[Authorize(Roles = "Admin")]
    public class TranslationsController : BaseController
    {
        private readonly TranslationService _tr;

        public TranslationsController(TranslationService tr) => _tr = tr;

       
        // POST api/translations
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] UpsertTranslationDto dto)
        {
            await _tr.UpsertAsync(dto.EntityType, dto.EntityId,
                                  dto.Field, dto.Lang, dto.Value);
            return Ok();
        }

        
        // DELETE api/translations/Center/5
        [HttpDelete("{entityType}/{entityId}")]
        public async Task<IActionResult> Delete(string entityType, int entityId)
        {
            await _tr.DeleteAllAsync(entityType, entityId);
            return NoContent();
        }
    }

    public class UpsertTranslationDto
    {
        public string EntityType { get; set; } = null!;
        public int EntityId { get; set; }
        public string Field { get; set; } = null!;
        public string Lang { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}