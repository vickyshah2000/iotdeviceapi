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
    public class ZoneController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public ZoneController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAllZones")]
        public async Task<IActionResult> GetAllZones()
        {
            var zones = await _ferrodbContext.Zones.ToListAsync();
            return Ok(zones);
        }

        [HttpGet("GetZoneById")]
        public async Task<IActionResult> GetZoneById(int id)
        {
            var zone = await _ferrodbContext.Zones.FindAsync(id);

            if (zone == null)
            {
                return NotFound("Zone not found");
            }

            return Ok(zone);
        }

        [HttpPost("CreateZone")]
        public async Task<IActionResult> CreateZone([FromBody] Zone zone)
        {
            if (zone == null)
            {
                return BadRequest("Invalid Zone data");
            }

            _ferrodbContext.Zones.Add(zone);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetZoneById), new { id = zone.ZoneId }, zone);
        }

        [HttpPut("UpdateZone")]
        public async Task<IActionResult> UpdateZone(int id, [FromBody] Zone updatedZone)
        {
            if (id != updatedZone.ZoneId)
            {
                return BadRequest("Zone ID mismatch");
            }

            var existingZone = await _ferrodbContext.Zones.FindAsync(id);
            if (existingZone == null)
            {
                return NotFound("Zone not found");
            }

            existingZone.Zonename = updatedZone.Zonename;

            _ferrodbContext.Zones.Update(existingZone);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteZone")]
        public async Task<IActionResult> DeleteZone(int id)
        {
            var zone = await _ferrodbContext.Zones.FindAsync(id);

            if (zone == null)
            {
                return NotFound("Zone not found");
            }

            _ferrodbContext.Zones.Remove(zone);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
