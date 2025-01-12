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
    public class AddressTypeController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public AddressTypeController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAllAddressTypes")]
        public async Task<IActionResult> GetAllAddressTypes()
        {
            var addressTypes = await _ferrodbContext.AddressTypes.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(addressTypes);
        }

        [HttpGet("GetAddressTypeById")]
        public async Task<IActionResult> GetAddressTypeById(long id)
        {
            var addressType = await _ferrodbContext.AddressTypes.FindAsync(id);

            if (addressType == null)
            {
                return NotFound("AddressType not found");
            }

            return Ok(addressType);
        }

        [HttpPost("CreateAddressType")]
        public async Task<IActionResult> CreateAddressType([FromBody] AddressType newAddressType)
        {
            if (newAddressType == null || string.IsNullOrEmpty(newAddressType.Name))
            {
                return BadRequest("Invalid AddressType data");
            }

            _ferrodbContext.AddressTypes.Add(newAddressType);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddressTypeById), new { id = newAddressType.Id }, newAddressType);
        }

        [HttpPut("UpdateAddressType")]
        public async Task<IActionResult> UpdateAddressType(long id, [FromBody] AddressType updatedAddressType)
        {
            if (id != updatedAddressType.Id)
            {
                return BadRequest("AddressType ID mismatch");
            }

            var existingAddressType = await _ferrodbContext.AddressTypes.FindAsync(id);
            if (existingAddressType == null)
            {
                return NotFound("AddressType not found");
            }

            existingAddressType.Name = updatedAddressType.Name;

            _ferrodbContext.AddressTypes.Update(existingAddressType);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteAddressType")]
        public async Task<IActionResult> DeleteAddressType(long id)
        {
            var addressType = await _ferrodbContext.AddressTypes.FindAsync(id);

            if (addressType == null)
            {
                return NotFound("AddressType not found");
            }

            _ferrodbContext.AddressTypes.Remove(addressType);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
