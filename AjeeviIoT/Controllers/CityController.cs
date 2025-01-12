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
    public class CityController : ControllerBase
    {
        private readonly ferrodbContext _context;

        public CityController(ferrodbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCitiesByStateId")]
        public async Task<IActionResult> GetCitiesByStateId(int stateId)
        {
            var cities = await _context.Cities
                .Where(c => c.StateId == stateId)
                .OrderByDescending(c => c.CityId)
                .ToListAsync();

            if (cities == null || !cities.Any())
            {
                return NotFound($"No cities found for StateId {stateId}");
            }

            return Ok(cities);
        }



        [HttpGet("GetAllCities")]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await _context.Cities.OrderByDescending(x => x.CityId).ToListAsync();
            return Ok(cities);
        }

        [HttpGet("GetCityById")]
        public async Task<IActionResult> GetCityById(int id)
        {
            var city = await _context.Cities.FindAsync(id);


            if (city == null)
            {
                return NotFound("City not found");
            }

            return Ok(city);
        }

        [HttpPost("CreateCity")]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityDTO newCityDTO)
        {
            if (newCityDTO == null || string.IsNullOrEmpty(newCityDTO.Cityname))
            {
                return BadRequest("Invalid city data");
            }

            // Map CreateCityDTO to City entity
            var newCity = new City
            {
                Cityname = newCityDTO.Cityname,
                StateId = newCityDTO.StateId
            };

            _context.Cities.Add(newCity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCityById), new { id = newCity.CityId }, newCity);
        }

        [HttpPut("UpdateCity")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] UpdateCityDTO updateCityDTO)
        {
            if (updateCityDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingCity = await _context.Cities.FindAsync(id);
            if (existingCity == null)
            {
                return NotFound("City not found.");
            }

            if (!string.IsNullOrEmpty(updateCityDTO.CityName))
                existingCity.Cityname = updateCityDTO.CityName;

            if (updateCityDTO.StateId.HasValue)
                existingCity.StateId = updateCityDTO.StateId;

            _context.Cities.Update(existingCity);
            await _context.SaveChangesAsync();

            return NoContent();
        }




        //[HttpPut("UpdateCity")]
        //public async Task<IActionResult> UpdateCity(int id, [FromBody] City updatedCity)
        //{
        //    if (id != updatedCity.CityId)
        //    {
        //        return BadRequest("City ID mismatch");
        //    }

        //    var existingCity = await _context.Cities.FindAsync(id);
        //    if (existingCity == null)
        //    {
        //        return NotFound("City not found");
        //    }

        //    existingCity.Cityname = updatedCity.Cityname;
        //    existingCity.StateId = updatedCity.StateId;

        //    _context.Cities.Update(existingCity);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteCity")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound("City not found");
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
