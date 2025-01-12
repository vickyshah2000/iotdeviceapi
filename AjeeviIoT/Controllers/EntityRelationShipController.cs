//using AjeeviIoT.EntitiesDTO;
//using AjeeviIoT.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//namespace AjeeviIoT.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EntityRelationShipController : ControllerBase
//    {
//        private readonly ferrodbContext _context;

//        public EntityRelationShipController(ferrodbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("CreateEntityRelationship")]
//        public async Task<IActionResult> CreateEntityRelationship([FromBody] CreateEntityRelationshipDTO createEntityRelationshipDTO)
//        {
//            if (createEntityRelationshipDTO == null)
//            {
//                return BadRequest("EntityRelationship data is null.");
//            }

//            var entityRelationship = new Entityrelationship
//            {
//                Entityid = createEntityRelationshipDTO.Entityid,
//                RelationshipId = createEntityRelationshipDTO.RelationshipId,
//                EntityroleId = createEntityRelationshipDTO.EntityroleId
//            };

//            _context.Entityrelationships.Add(entityRelationship);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetEntityRelationshipById), new { id = entityRelationship.Id }, entityRelationship);
//        }


//        [HttpGet("GetAllEntityRelationships")]
//        public async Task<IActionResult> GetAllEntityRelationships()
//        {
//            var entityRelationships = await _context.Entityrelationships.OrderByDescending(x => x.Id).ToListAsync();
//            return Ok(entityRelationships);
//        }

//        [HttpGet("GetEntityRelationshipByentityid")]
//        public async Task<IActionResult> GetEntityRelationshipById(int entityid)
//        {
            
//        }

//        [HttpPut("UpdateEntityRelationship")]
//        public async Task<IActionResult> UpdateEntityRelationship(int id, [FromBody] Entityrelationship updatedEntityRelationship)
//        {
//            if (updatedEntityRelationship == null)
//            {
//                return BadRequest("EntityRelationship data is null.");
//            }

//            var entityRelationship = await _context.Entityrelationships.FindAsync(id);

//            if (entityRelationship == null)
//            {
//                return NotFound($"EntityRelationship with Id {id} not found.");
//            }

//            entityRelationship.Entityid = updatedEntityRelationship.Entityid;
//            entityRelationship.RelationshipId = updatedEntityRelationship.RelationshipId;
//            entityRelationship.EntityroleId = updatedEntityRelationship.EntityroleId;

//            _context.Entityrelationships.Update(entityRelationship);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        [HttpDelete("DeleteEntityRelationship")]
//        public async Task<IActionResult> DeleteEntityRelationship(int id)
//        {
//            var entityRelationship = await _context.Entityrelationships.FindAsync(id);

//            if (entityRelationship == null)
//            {
//                return NotFound($"EntityRelationship with Id {id} not found.");
//            }

//            _context.Entityrelationships.Remove(entityRelationship);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
