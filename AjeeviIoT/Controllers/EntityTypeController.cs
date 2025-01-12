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
    public class EntityTypeController : ControllerBase
    {
        private readonly ferrodbContext _context;

        public EntityTypeController(ferrodbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateEntityType")]
        public async Task<IActionResult> CreateEntityType([FromBody] CreateEntityTypeDTO createEntityTypeDTO)
        {
            if (createEntityTypeDTO == null)
            {
                return BadRequest("EntityType data is null.");
            }

            var entityType = new Entitytype
            {
                EntityTypeName = createEntityTypeDTO.EntityTypeName
            };

            _context.Entitytypes.Add(entityType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntityTypeById), new { id = entityType.TypeId }, entityType);
        }


        [HttpGet("GetAllEntityTypes")]
        public async Task<IActionResult> GetAllEntityTypes()
        {
            var entityTypes = await _context.Entitytypes.OrderByDescending(x => x.TypeId).ToListAsync();
            return Ok(entityTypes);
        }

        [HttpGet("GetEntityTypeById")]
        public async Task<IActionResult> GetEntityTypeById(int id)
        {
            var entityType = await _context.Entitytypes.FindAsync(id);

            if (entityType == null)
            {
                return NotFound($"EntityType with Id {id} not found.");
            }

            return Ok(entityType);
        }


        [HttpPut("UpdateEntityType")]
        public async Task<IActionResult> UpdateEntityType(int id, [FromBody] UpdateEntityTypeDTO updateEntityTypeDTO)
        {
            if (updateEntityTypeDTO == null)
            {
                return BadRequest("EntityType data is null.");
            }

            var entityType = await _context.Entitytypes.FindAsync(id);
            if (entityType == null)
            {
                return NotFound($"EntityType with Id {id} not found.");
            }

            if (!string.IsNullOrEmpty(updateEntityTypeDTO.EntityTypeName))
            {
                entityType.EntityTypeName = updateEntityTypeDTO.EntityTypeName;
            }

            _context.Entitytypes.Update(entityType);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //[HttpPut("UpdateEntityType")]
        //public async Task<IActionResult> UpdateEntityType(int id, [FromBody] Entitytype updatedEntityType)
        //{
        //    if (updatedEntityType == null)
        //    {
        //        return BadRequest("EntityType data is null.");
        //    }

        //    var entityType = await _context.Entitytypes.FindAsync(id);

        //    if (entityType == null)
        //    {
        //        return NotFound($"EntityType with Id {id} not found.");
        //    }

        //    entityType.EntityTypeName = updatedEntityType.EntityTypeName;

        //    _context.Entitytypes.Update(entityType);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteEntityType")]
        public async Task<IActionResult> DeleteEntityType(int id)
        {
            var entityType = await _context.Entitytypes.FindAsync(id);

            if (entityType == null)
            {
                return NotFound($"EntityType with Id {id} not found.");
            }

            _context.Entitytypes.Remove(entityType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
