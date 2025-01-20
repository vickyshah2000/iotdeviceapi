using AjeeviIoT.EntitiesDTO;
using AjeeviIoT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;


namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbcontext;

        public DesignationController(ferrodbContext ferrodbcontext)
        {
            _ferrodbcontext = ferrodbcontext;
        }

        [HttpGet("GetAllDesignation")]
        public async Task<IActionResult> GetDesignations(int? pageNumber, int? pageSize)
        {
            try
            {
                int currentPage = pageNumber ?? 1;
                int currentPageSize = pageSize ?? 100;

                var design = await _ferrodbcontext.Designations.OrderByDescending(x => x.DsId).Skip((currentPage - 1) * currentPageSize).Take(currentPageSize).ToListAsync();

                var designation = design.Select(a => new Designation
                {
                    DsId = a.DsId,
                    DsName = a.DsName,
                    DepartmentId = a.DsId
                });

                var totalCount = await _ferrodbcontext.Designations.CountAsync();

                var paginationResult = new
                {
                    TotalCount = totalCount,
                    PageNumber = currentPage,
                    PageSize = currentPageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)currentPageSize),
                    Designation = designation
                };

                return Ok(paginationResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Designation>> GetDesignation(int id)
        //{
        //    var designation = await _ferrodbcontext.Designations.FindAsync(id);

        //    if (designation == null)
        //    {
        //        return NotFound();
        //    }

        //    return designation;
        //}

        [HttpPost("CreateDesignation")]
        public async Task<ActionResult<Designation>> CreateDesignation(CreateDesignationDTO designation)
        {
            try
            {
                var dsg = new Designation
                {
                    DsName = designation.DsName,
                    DepartmentId = designation.DepartmentId
                };

                await _ferrodbcontext.Designations.AddAsync(dsg);
                await _ferrodbcontext.SaveChangesAsync();
                return Ok(designation);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }



        }

        //[HttpPut("EditDesignation")]
        //public async Task<IActionResult> UpdateDesignation(int id, Designation designation)
        //{
        //    if (id != designation.DsId)
        //    {
        //        return BadRequest("Designation ID mismatch.");
        //    }



            
        //}

        [HttpDelete("deleteDesignation")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            var designation = await _ferrodbcontext.Designations.FindAsync(id);
            if (designation == null)
            {
                return NotFound();
            }

            _ferrodbcontext.Designations.Remove(designation);
            await _ferrodbcontext.SaveChangesAsync();

            return NoContent();
        }

        private bool DesignationExists(int id)
        {
            return _ferrodbcontext.Designations.Any(e => e.DsId == id);
        }
    }
}
