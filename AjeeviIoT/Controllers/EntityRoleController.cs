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
    public class EntityRoleController : ControllerBase
    {
        private readonly ferrodbContext _context;

        public EntityRoleController(ferrodbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateEntityRole")]
        public async Task<IActionResult> CreateEntityRole([FromBody] CreateEntityRoleDTO createEntityRoleDTO)
        {
            if (createEntityRoleDTO == null)
            {
                return BadRequest("EntityRole data is null.");
            }

            var entityRole = new Entityrole
            {
                Rolename = createEntityRoleDTO.Rolename
            };

            _context.Entityroles.Add(entityRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntityRoleById), new { id = entityRole.RoleId }, entityRole);
        }


        [HttpGet("GetAllEntityRoles")]
        public async Task<IActionResult> GetAllEntityRoles()
        {
            var entityRoles = await _context.Entityroles.OrderByDescending(x => x.RoleId).ToListAsync();
            return Ok(entityRoles);
        }

        [HttpGet("GetEntityRoleById")]
        public async Task<IActionResult> GetEntityRoleById(int id)
        {
            var entityRole = await _context.Entityroles.FindAsync(id);

            if (entityRole == null)
            {
                return NotFound($"EntityRole with Id {id} not found.");
            }

            return Ok(entityRole);
        }


        [HttpPut("UpdateEntityRole")]
        public async Task<IActionResult> UpdateEntityRole(int id, [FromBody] UpdateEntityRoleDTO updateEntityRoleDTO)
        {
            if (updateEntityRoleDTO == null)
            {
                return BadRequest("EntityRole data is null.");
            }

            var entityRole = await _context.Entityroles.FindAsync(id);
            if (entityRole == null)
            {
                return NotFound($"EntityRole with Id {id} not found.");
            }

            if (!string.IsNullOrEmpty(updateEntityRoleDTO.Rolename))
            {
                entityRole.Rolename = updateEntityRoleDTO.Rolename;
            }

            _context.Entityroles.Update(entityRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        //[HttpPut("UpdateEntityRole")]
        //public async Task<IActionResult> UpdateEntityRole(int id, [FromBody] Entityrole updatedEntityRole)
        //{
        //    if (updatedEntityRole == null)
        //    {
        //        return BadRequest("EntityRole data is null.");
        //    }

        //    var entityRole = await _context.Entityroles.FindAsync(id);

        //    if (entityRole == null)
        //    {
        //        return NotFound($"EntityRole with Id {id} not found.");
        //    }

        //    entityRole.Rolename = updatedEntityRole.Rolename;

        //    _context.Entityroles.Update(entityRole);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteEntityRole")]
        public async Task<IActionResult> DeleteEntityRole(int id)
        {
            var entityRole = await _context.Entityroles.FindAsync(id);

            if (entityRole == null)
            {
                return NotFound($"EntityRole with Id {id} not found.");
            }

            _context.Entityroles.Remove(entityRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
