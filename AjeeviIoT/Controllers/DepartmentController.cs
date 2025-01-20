using AjeeviIoT.EntitiesDTO;
using AjeeviIoT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public DepartmentController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAllDepartment")]
        public async Task<IActionResult> GetAllDepartment(int? pageNumber, int? pageSize)
        {

            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 100;

            var department = await _ferrodbContext.Departments.OrderByDescending(x => x.DId).Skip((currentPage - 1) * currentPageSize).Take(currentPageSize).ToListAsync();
            if (department == null)
            {
                return NotFound();
            }

            var departments = department.Select(a => new Department
            {
                DId = a.DId,
                DName = a.DName
            }).ToList();

            var totalCount = await _ferrodbContext.Departments.CountAsync();


            var paginationResult = new
            {
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = currentPageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)currentPageSize),
                Department = departments
            };

            return Ok(paginationResult);
        }

        [HttpPost("CreateAddress")]
        public async Task<IActionResult> CreateAddress([FromBody] createDepartmentDTO departmentdto)
        {
            if (departmentdto == null)
            {
                return BadRequest("Invalid Department data");
            }

            var department = new Department
            {
               DName = departmentdto.DName
            };
             
            await _ferrodbContext.Departments.AddAsync(department);
            await _ferrodbContext.SaveChangesAsync();

            return Ok(department);
        }

    }
}
