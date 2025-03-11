using AjeeviIoT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public OrganizationController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet]
        public IActionResult GetAllOrganisations()
        {
            try
            {
                var organisations = _ferrodbContext.Organisations.OrderByDescending(o => o.Id).ToList();
                return Ok(organisations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching organisations.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOrganisationById(int id)
        {
            try
            {
                var organisation = _ferrodbContext.Organisations.FirstOrDefault(o => o.Id == id);
                if (organisation == null)
                    return NotFound(new { Message = "Organisation not found." });

                return Ok(organisation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the organisation.", Error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateOrganisation([FromBody] Organisation organisation)
        {
            try
            {
                if (organisation == null)
                {
                    return BadRequest(new { Message = "Invalid organisation data." });
                }

                if (string.IsNullOrWhiteSpace(organisation.Name))
                {
                    return BadRequest(new { Message = "Name is required." });
                }

                var newOrganisation = new Organisation
                {
                    Name = organisation.Name,
                    ShortName = organisation.ShortName,
                    LongName = organisation.LongName,
                    TradeName = organisation.TradeName,
                    OrgTypeId = organisation.OrgTypeId,
                    OrgRoleId = organisation.OrgRoleId,
                    Remarks = organisation.Remarks,
                    Entityid = organisation.Entityid
                };

                _ferrodbContext.Organisations.Add(newOrganisation);
                _ferrodbContext.SaveChanges();

                return Ok(newOrganisation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the organisation.", Error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public IActionResult UpdateOrganisation(int id, [FromBody] Organisation updatedOrganisation)
        {
            try
            {
                var organisation = _ferrodbContext.Organisations.FirstOrDefault(o => o.Id == id);
                if (organisation == null)
                    return NotFound(new { Message = "Organisation not found." });

                organisation.Name = updatedOrganisation.Name;
                organisation.ShortName = updatedOrganisation.ShortName;
                organisation.LongName = updatedOrganisation.LongName;
                organisation.TradeName = updatedOrganisation.TradeName;
                organisation.OrgTypeId = updatedOrganisation.OrgTypeId;
                organisation.OrgRoleId = updatedOrganisation.OrgRoleId;
                organisation.Remarks = updatedOrganisation.Remarks;
                organisation.Entityid = updatedOrganisation.Entityid;

                _ferrodbContext.SaveChanges();

                return Ok(new { Message = "Organisation updated successfully.", Data = organisation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the organisation.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrganisation(int id)
        {
            try
            {
                var organisation = _ferrodbContext.Organisations.FirstOrDefault(o => o.Id == id);
                if (organisation == null)
                    return NotFound(new { Message = "Organisation not found." });

                _ferrodbContext.Organisations.Remove(organisation);
                _ferrodbContext.SaveChanges();

                return Ok(new { Message = "Organisation deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the organisation.", Error = ex.Message });
            }
        }
    }
}
