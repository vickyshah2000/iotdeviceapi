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
    public class WardController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public WardController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetWardsByStateId")]
        public async Task<IActionResult> GetWardsByStateId(int stateId)
        {
            var wards = await _ferrodbContext.Wards
                .Where(w => w.Stateid == stateId) 
                .OrderBy(w => w.Wardname)   
                .ToListAsync();

            if (wards == null || !wards.Any())
            {
                return NotFound($"No wards found for StateId {stateId}");
            }

            var wardDtos = wards.Select(w => new WardDto
            {
                WardId = w.WardId,
                Wardname = w.Wardname,
                Stateid = w.Stateid
            }).ToList();

            return Ok(wardDtos);
        }



        [HttpGet("GetAllWards")]
        public async Task<IActionResult> GetAllWards()
        {
            var wards = await _ferrodbContext.Wards.ToListAsync();
            return Ok(wards);
        }

        [HttpGet("GetWardById")]
        public async Task<IActionResult> GetWardById(int id)
        {
            var ward = await _ferrodbContext.Wards.FindAsync(id);

            if (ward == null)
            {
                return NotFound("Ward not found");
            }

            return Ok(ward);
        }

        [HttpPost("CreateWard")]
        public async Task<IActionResult> CreateWard([FromBody] CreateWardDTO createWardDTO)
        {
            if (createWardDTO == null)
            {
                return BadRequest("Invalid Ward data");
            }

            // Create a new Ward object using the data from the DTO
            var ward = new Ward
            {
                Wardname = createWardDTO.Wardname,
                Stateid = createWardDTO.Stateid
            };

            _ferrodbContext.Wards.Add(ward);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWardById), new { id = ward.WardId }, ward);
        }

        [HttpPut("UpdateWard")]
        public async Task<IActionResult> UpdateWard(int id, [FromBody] UpdateWardDTO updateWardDTO)
        {
            if (id != updateWardDTO.WardId)
            {
                return BadRequest("Ward ID mismatch");
            }

            var existingWard = await _ferrodbContext.Wards.FindAsync(id);
            if (existingWard == null)
            {
                return NotFound("Ward not found");
            }

            try
            {
                existingWard.Wardname = updateWardDTO.Wardname;
                existingWard.Stateid = updateWardDTO.Stateid;

                _ferrodbContext.Wards.Update(existingWard);
                await _ferrodbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        //[HttpPut("UpdateWard")]
        //public async Task<IActionResult> UpdateWard(int id, [FromBody] Ward updatedWard)
        //{
        //    if (id != updatedWard.WardId)
        //    {
        //        return BadRequest("Ward ID mismatch");
        //    }

        //    var existingWard = await _ferrodbContext.Wards.FindAsync(id);
        //    if (existingWard == null)
        //    {
        //        return NotFound("Ward not found");
        //    }

        //    existingWard.Wardname = updatedWard.Wardname;
        //    existingWard.Stateid = updatedWard.Stateid;

        //    _ferrodbContext.Wards.Update(existingWard);
        //    await _ferrodbContext.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteWard")]
        public async Task<IActionResult> DeleteWard(int id)
        {
            var ward = await _ferrodbContext.Wards.FindAsync(id);

            if (ward == null)
            {
                return NotFound("Ward not found");
            }

            _ferrodbContext.Wards.Remove(ward);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
