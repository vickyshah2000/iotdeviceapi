using AjeeviIoT.EntitiesDTO;
using AjeeviIoT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDeviceController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public EmployeeDeviceController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAllEmployeeDevice")]
        public async Task<IActionResult> GetAllEmployeeDevice(int pageNumber = 1, int pageSize = 100)
        {
            var eyd = await _ferrodbContext.Employeedevices.OrderByDescending(X =>  X.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            if (eyd.Count > 0 )
            {
                return NotFound();
            }

            var empdevices = eyd.Select(a => new Employeedevice
            {
                Id = a.Id,
                PersonId = a.PersonId,
                DeviceId = a.DeviceId,
                Assigntime = a.Assigntime,
                Status = a.Status
            }).ToList();

            var totalCount = await _ferrodbContext.Employeedevices.CountAsync();

            var paginationResult = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Employeedevice = empdevices
            };

            return Ok(paginationResult);

        }

        [HttpPost("CreateEmployeeDevice")]
        public async Task<IActionResult> CreateEmployeeDevice(CreateEmployeeDeviceDTO createEmployee)
        {
            if (createEmployee == null)
            {
                return NotFound();
            }

            var empd = new Employeedevice
            {
                PersonId = createEmployee.PersonId,
                DeviceId = createEmployee.DeviceId,
                Assigntime = createEmployee.Assigntime,
                Status = createEmployee.Status
            };

            await _ferrodbContext.Employeedevices.AddAsync(empd);
            _ferrodbContext.SaveChanges();
            return Ok(createEmployee);
        }

    }
}
