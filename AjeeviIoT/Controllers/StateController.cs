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
    public class StateController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public StateController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetStatesByCountryId")]
        public async Task<IActionResult> GetStatesByCountryId(int countryId)
        {
            var states = await _ferrodbContext.States
                .Where(s => s.CountryId == countryId)  
                .OrderByDescending(s => s.StateId)     
                .Include(s => s.Country)                
                .ToListAsync();

            if (states == null || !states.Any())
            {
                return NotFound($"No states found for CountryId {countryId}");
            }

            var stateDtos = states.Select(s => new StateDto
            {
                StateId = s.StateId,
                Statename = s.Statename,
                CountryId = countryId
            }).ToList();

            return Ok(stateDtos);
        }



        [HttpGet("GetAllStates")]
        public async Task<IActionResult> GetAllStates()
        {
            var states = await _ferrodbContext.States.OrderByDescending(x => x.StateId).Include(s => s.Country).ToListAsync();

            var stateDtos = states.Select(s => new StateDto
            {
                StateId = s.StateId,
                Statename = s.Statename
            }).ToList();

            return Ok(stateDtos);
        }


        [HttpGet("GetStateById")]
        public async Task<IActionResult> GetStateById(int id)
        {
            var state = await _ferrodbContext.States.FirstAsync(s => s.StateId == id);

            if (state == null)
            {
                return NotFound("State not found");
            }

            return Ok(state);
        }

        [HttpPost("CreateState")]
        public async Task<IActionResult> CreateState([FromBody] CreateStateDTO createStateDTO)
        {
            if (createStateDTO == null)
            {
                return BadRequest("Invalid State data");
            }

            var state = new State
            {
                Statename = createStateDTO.Statename,
                CountryId = createStateDTO.CountryId
            };

            _ferrodbContext.States.Add(state);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStateById), new { id = state.StateId }, state);
        }

        [HttpPut("UpdateState")]
        public async Task<IActionResult> UpdateState(int id, [FromBody] UpdateStateDTO updateStateDTO)
        {
            if (id != updateStateDTO.StateId)
            {
                return BadRequest("State ID mismatch");
            }

            var existingState = await _ferrodbContext.States.FindAsync(id);
            if (existingState == null)
            {
                return NotFound("State not found");
            }

            try
            {
                existingState.Statename = updateStateDTO.Statename;
                existingState.CountryId = updateStateDTO.CountryId;

                _ferrodbContext.States.Update(existingState);
                await _ferrodbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        //[HttpPut("UpdateState")]
        //public async Task<IActionResult> UpdateState(int id, [FromBody] State updatedState)
        //{
        //    if (id != updatedState.StateId)
        //    {
        //        return BadRequest("State ID mismatch");
        //    }

        //    var existingState = await _ferrodbContext.States.FindAsync(id);
        //    if (existingState == null)
        //    {
        //        return NotFound("State not found");
        //    }

        //    existingState.Statename = updatedState.Statename;
        //    existingState.CountryId = updatedState.CountryId;

        //    _ferrodbContext.States.Update(existingState);
        //    await _ferrodbContext.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteState")]
        public async Task<IActionResult> DeleteState(int id)
        {
            var state = await _ferrodbContext.States.FindAsync(id);

            if (state == null)
            {
                return NotFound("State not found");
            }

            _ferrodbContext.States.Remove(state);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
