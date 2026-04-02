using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Centers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CentersController : ControllerBase
    {
        private readonly ICenterService _centerService;

        public CentersController(ICenterService centerService)
        {
            _centerService = centerService;
        }

        // GET api/centers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var centers = await _centerService.GetAllCentersAsync();
            return Ok(centers);
        }

        // GET api/centers/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var center = await _centerService.GetCenterByIdAsync(id);
            return center is null ? NotFound() : Ok(center);
        }

        // POST api/centers
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCenterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _centerService.CreateCenterAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/centers/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCenterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _centerService.UpdateCenterAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        // DELETE api/centers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _centerService.DeleteCenterAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        // PUT api/centers/5/social-links
        [HttpPut("{id:int}/social-links")]
        public async Task<IActionResult> UpdateSocialLinks(int id, [FromBody] SocialLinksDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _centerService.UpdateSocialLinksAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }


        // PUT api/centers/5/settings/logo
        [HttpPut("{id:int}/settings/logo")]
        public async Task<IActionResult> UpdateLogo(int id, [FromBody] UpdateLogoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _centerService.UpdateLogoAsync(id, dto.LogoUrl);
            return updated is null ? NotFound() : Ok(updated);
        }


        // PUT api/centers/5/settings
        [HttpPut("{id:int}/settings")]
        public async Task<IActionResult> UpdateSettings(int id, [FromBody] CenterSettingsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var updated = await _centerService.UpdateSettingsAsync(id, dto, userId);
            return updated is null ? NotFound() : Ok(updated);
        }
    }
}

