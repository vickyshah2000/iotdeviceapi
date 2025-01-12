using AjeeviIoT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class ContactTypeController : ControllerBase
    {
        private readonly ferrodbContext _context;

        public ContactTypeController(ferrodbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllContactTypes()
        {
            var contactTypes = await _context.Contacttypes.ToListAsync();
            return Ok(contactTypes);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetContactTypeById(int id)
        {
            var contactType = await _context.Contacttypes.FindAsync(id);

            if (contactType == null)
            {
                return NotFound("Contact type not found");
            }

            return Ok(contactType);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateContactType([FromBody] Contacttype newContactType)
        {
            if (newContactType == null || string.IsNullOrEmpty(newContactType.Contacttypename))
            {
                return BadRequest("Invalid contact type data");
            }

            _context.Contacttypes.Add(newContactType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactTypeById), new { id = newContactType.ContactTypeId }, newContactType);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateContactType(int id, [FromBody] Contacttype updatedContactType)
        {
            if (id != updatedContactType.ContactTypeId)
            {
                return BadRequest("Contact type ID mismatch");
            }

            var existingContactType = await _context.Contacttypes.FindAsync(id);
            if (existingContactType == null)
            {
                return NotFound("Contact type not found");
            }

            existingContactType.Contacttypename = updatedContactType.Contacttypename;

            _context.Contacttypes.Update(existingContactType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteContactType(int id)
        {
            var contactType = await _context.Contacttypes.FindAsync(id);

            if (contactType == null)
            {
                return NotFound("Contact type not found");
            }

            _context.Contacttypes.Remove(contactType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
