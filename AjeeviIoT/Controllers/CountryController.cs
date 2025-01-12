using AjeeviIoT.EntitiesDTO;
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
    public class CountryController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public CountryController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _ferrodbContext.Countries.OrderByDescending(x => x.CountryId).ToListAsync();
            return Ok(countries);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            var country = await _ferrodbContext.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound("Country not found");
            }

            return Ok(country);
        }

        [HttpPost("CreateCountry")]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO newCountryDTO)
        {
            if (newCountryDTO == null || string.IsNullOrEmpty(newCountryDTO.Name))
            {
                return BadRequest("Invalid country data");
            }

            // Map CreateCountryDTO to Country entity
            var newCountry = new Country
            {
                Name = newCountryDTO.Name
            };

            _ferrodbContext.Countries.Add(newCountry);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCountryById), new { id = newCountry.CountryId }, newCountry);
        }

        [HttpPut("UpdateCountry")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO updateCountryDTO)
        {
            if (updateCountryDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingCountry = await _ferrodbContext.Countries.FindAsync(id);
            if (existingCountry == null)
            {
                return NotFound("Country not found.");
            }

            if (!string.IsNullOrEmpty(updateCountryDTO.Name))
                existingCountry.Name = updateCountryDTO.Name;

            _ferrodbContext.Countries.Update(existingCountry);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }



        //[HttpPut("UpdateCountry")]
        //public async Task<IActionResult> UpdateCountry(int id, [FromBody] Country updatedCountry)
        //{
        //    if (id != updatedCountry.CountryId)
        //    {
        //        return BadRequest("Country ID mismatch");
        //    }

        //    var existingCountry = await _ferrodbContext.Countries.FindAsync(id);
        //    if (existingCountry == null)
        //    {
        //        return NotFound("Country not found");
        //    }

        //    existingCountry.Name = updatedCountry.Name;

        //    _ferrodbContext.Countries.Update(existingCountry);
        //    await _ferrodbContext.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteCountry")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _ferrodbContext.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound("Country not found");
            }

            _ferrodbContext.Countries.Remove(country);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
