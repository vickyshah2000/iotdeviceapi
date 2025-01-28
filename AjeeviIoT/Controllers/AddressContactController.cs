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
    public class AddressContactController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public AddressContactController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAllAddressContacts")]
        public async Task<IActionResult> GetAllAddressContacts()
        {
            var addressContacts = await _ferrodbContext.Addresscontacts.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(addressContacts);
        }

        [HttpGet("GetAddressContactById")]
        public async Task<IActionResult> GetAddressContactById(int id)
        {
            var addressContact = await _ferrodbContext.Addresscontacts.FindAsync(id);
            //testing

            if (addressContact == null)
            {
                return NotFound("AddressContact not found");
            }

            return Ok(addressContact);
        }

        [HttpPost("CreateAddressContact")]
        public async Task<IActionResult> CreateAddressContact([FromBody] Addresscontact newAddressContact)
        {
            if (newAddressContact == null)
            {
                return BadRequest("Invalid AddressContact data");
            }

            _ferrodbContext.Addresscontacts.Add(newAddressContact);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddressContactById), new { id = newAddressContact.Id }, newAddressContact);
        }

        [HttpPut("UpdateAddressContact")]
        public async Task<IActionResult> UpdateAddressContact(int id, [FromBody] Addresscontact updatedAddressContact)
        {
            if (id != updatedAddressContact.Id)
            {
                return BadRequest("AddressContact ID mismatch");
            }

            var existingAddressContact = await _ferrodbContext.Addresscontacts.FindAsync(id);

            if (existingAddressContact == null)
            {
                return NotFound("AddressContact not found");
            }

            existingAddressContact.Addressid = updatedAddressContact.Addressid;
            existingAddressContact.Personid = updatedAddressContact.Personid;

            _ferrodbContext.Addresscontacts.Update(existingAddressContact);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteAddressContact")]
        public async Task<IActionResult> DeleteAddressContact(int id)
        {
            var addressContact = await _ferrodbContext.Addresscontacts.FindAsync(id);

            if (addressContact == null)
            {
                return NotFound("AddressContact not found");
            }

            _ferrodbContext.Addresscontacts.Remove(addressContact);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
