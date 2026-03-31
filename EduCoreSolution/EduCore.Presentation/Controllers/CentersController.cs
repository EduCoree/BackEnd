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
    }
}

