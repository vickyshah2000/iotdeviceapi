using AjeeviIoT.EntitiesDTO;
using AjeeviIoT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class RegionController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public RegionController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAllRegions")]
        public async Task<IActionResult> GetAllRegions()
        {
            var regions = await _ferrodbContext.Regions.OrderByDescending(x => x.RegionId).ToListAsync();
            return Ok(regions);
        }

        [HttpGet("GetRegionById")]
        public async Task<IActionResult> GetRegionById(int id)
        {
            var region = await _ferrodbContext.Regions.FindAsync(id);

            if (region == null)
            {
                return NotFound("Region not found");
            }

            return Ok(region);
        }

        [HttpPost("CreateRegion")]
        public async Task<IActionResult> CreateRegion([FromBody] CreateRegionDTO createRegionDTO)
        {
            if (createRegionDTO == null)
            {
                return BadRequest("Invalid Region data");
            }

            var region = new Region
            {
                Regionname = createRegionDTO.Regionname
            };

            _ferrodbContext.Regions.Add(region);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRegionById), new { id = region.RegionId }, region);
        }

        [HttpPut("UpdateRegion")]
        public async Task<IActionResult> UpdateRegion(int id, [FromBody] UpdateRegionDTO updateRegionDTO)
        {
            if (id != updateRegionDTO.RegionId)
            {
                return BadRequest("Region ID mismatch");
            }

            var existingRegion = await _ferrodbContext.Regions.FindAsync(id);
            if (existingRegion == null)
            {
                return NotFound("Region not found");
            }

            try
            {
                existingRegion.Regionname = updateRegionDTO.Regionname;

                _ferrodbContext.Regions.Update(existingRegion);
                await _ferrodbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        //[HttpPut("UpdateRegion")]
        //public async Task<IActionResult> UpdateRegion(int id, [FromBody] Region updatedRegion)
        //{
        //    if (id != updatedRegion.RegionId)
        //    {
        //        return BadRequest("Region ID mismatch");
        //    }

        //    var existingRegion = await _ferrodbContext.Regions.FindAsync(id);
        //    if (existingRegion == null)
        //    {
        //        return NotFound("Region not found");
        //    }

        //    existingRegion.Regionname = updatedRegion.Regionname;

        //    _ferrodbContext.Regions.Update(existingRegion);
        //    await _ferrodbContext.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteRegion")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            var region = await _ferrodbContext.Regions.FindAsync(id);

            if (region == null)
            {
                return NotFound("Region not found");
            }

            _ferrodbContext.Regions.Remove(region);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
