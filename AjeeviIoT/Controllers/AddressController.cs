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
    public class AddressController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;

        public AddressController(ferrodbContext ferrodbContext)
        {
            _ferrodbContext = ferrodbContext;
        }

        [HttpGet("GetAllAddresses")]
        public async Task<IActionResult> GetAllAddresses(int pageNumber = 1, int pageSize = 100)
        {
            var addresses = await _ferrodbContext.Addresses.OrderByDescending(x => x.Id)
                .Include(a => a.City)
                .Include(a => a.State)
                .Include(a => a.Country)
                .Include(a => a.Region)
                .Include(a => a.Ward)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var addressDtos = addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Addressline1 = a.Addressline1,
                Addressline2 = a.Addressline2,
                Pincode = a.Pincode,
                CityName = a.City?.Cityname,
                StateName = a.State?.Statename,
                CountryName = a.Country?.Name,
                RegionName = a.Region?.Regionname,
                WardName = a.Ward?.Wardname,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                Remarks = a.Remarks
            }).ToList();

            var totalCount = await _ferrodbContext.Addresses.CountAsync();

            var paginationResult = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Addresses = addressDtos
            };

            return Ok(paginationResult);
        }

        [HttpGet("GetAddressById")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _ferrodbContext.Addresses
                .Where(a => a.Id == id)
                .Include(a => a.City)
                .Include(a => a.State)
                .Include(a => a.Country)
                .Include(a => a.Region)
                .Include(a => a.Ward)
                .FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound("Address not found");
            }

            var addressDto = new AddressDto
            {
                Id = address.Id,
                Addressline1 = address.Addressline1,
                Addressline2 = address.Addressline2,
                Pincode = address.Pincode,
                CityName = address.City?.Cityname,
                StateName = address.State?.Statename,
                CountryName = address.Country?.Name,
                RegionName = address.Region?.Regionname,
                WardName = address.Ward?.Wardname,
                Latitude = address.Latitude,
                Longitude = address.Longitude,
                Remarks = address.Remarks
            };

            return Ok(addressDto);
        }




        //[HttpGet("GetAddressById")]
        //public async Task<IActionResult> GetAddressById(int id)
        //{
        //    var address = await _ferrodbContext.Addresses.FindAsync(id);


        //    if (address == null)
        //    {
        //        return NotFound("Address not found");
        //    }

        //    return Ok(address);
        //}

        [HttpPost("CreateAddress")]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDTO newAddressDTO)
        {
            if (newAddressDTO == null || string.IsNullOrEmpty(newAddressDTO.Addressline1) || string.IsNullOrEmpty(newAddressDTO.Pincode))
            {
                return BadRequest("Invalid Address data");
            }

            var newAddress = new Address
            {
                Addressline1 = newAddressDTO.Addressline1,
                Addressline2 = newAddressDTO.Addressline2,
                Pincode = newAddressDTO.Pincode,
                CityId = newAddressDTO.CityId,
                Stateid = newAddressDTO.Stateid,
                CountryId = newAddressDTO.CountryId,
                Latitude = newAddressDTO.Latitude,
                Longitude = newAddressDTO.Longitude,
                Remarks = newAddressDTO.Remarks
            };

            _ferrodbContext.Addresses.Add(newAddress);
            await _ferrodbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddressById), new { id = newAddress.Id }, newAddress);
        }


        [HttpPut("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDTO updateAddressDTO)
        {
            if (updateAddressDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingAddress = await _ferrodbContext.Addresses.FindAsync(id);
            if (existingAddress == null)
            {
                return NotFound("Address not found.");
            }

            if (!string.IsNullOrEmpty(updateAddressDTO.AddressLine1))
                existingAddress.Addressline1 = updateAddressDTO.AddressLine1;

            if (!string.IsNullOrEmpty(updateAddressDTO.AddressLine2))
                existingAddress.Addressline2 = updateAddressDTO.AddressLine2;

            if (!string.IsNullOrEmpty(updateAddressDTO.Pincode))
                existingAddress.Pincode = updateAddressDTO.Pincode;

            if (updateAddressDTO.CityId.HasValue)
                existingAddress.CityId = updateAddressDTO.CityId;

            if (updateAddressDTO.StateId.HasValue)
                existingAddress.Stateid = updateAddressDTO.StateId;

            if (updateAddressDTO.CountryId.HasValue)
                existingAddress.CountryId = updateAddressDTO.CountryId;

            if (updateAddressDTO.RegionId.HasValue)
                existingAddress.Regionid = updateAddressDTO.RegionId;

            if (updateAddressDTO.WardId.HasValue)
                existingAddress.Wardid = updateAddressDTO.WardId;

            if (updateAddressDTO.ZoneId.HasValue)
                existingAddress.Zoneid = updateAddressDTO.ZoneId;

            if (!string.IsNullOrEmpty(updateAddressDTO.Latitude))
                existingAddress.Latitude = updateAddressDTO.Latitude;

            if (!string.IsNullOrEmpty(updateAddressDTO.Longitude))
                existingAddress.Longitude = updateAddressDTO.Longitude;

            if (!string.IsNullOrEmpty(updateAddressDTO.Remarks))
                existingAddress.Remarks = updateAddressDTO.Remarks;

            _ferrodbContext.Addresses.Update(existingAddress);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }



        //[HttpPut("UpdateAddress")]
        //public async Task<IActionResult> UpdateAddress(int id, [FromBody] Address updatedAddress)
        //{
        //    if (id != updatedAddress.Id)
        //    {
        //        return BadRequest("Address ID mismatch");
        //    }

        //    var existingAddress = await _ferrodbContext.Addresses.FindAsync(id);
        //    if (existingAddress == null)
        //    {
        //        return NotFound("Address not found");
        //    }

        //    existingAddress.Addressline1 = updatedAddress.Addressline1;
        //    existingAddress.Addressline2 = updatedAddress.Addressline2;
        //    existingAddress.Pincode = updatedAddress.Pincode;
        //    existingAddress.CityId = updatedAddress.CityId;
        //    existingAddress.Stateid = updatedAddress.Stateid;
        //    existingAddress.CountryId = updatedAddress.CountryId;
        //    existingAddress.Regionid = updatedAddress.Regionid;
        //    existingAddress.Wardid = updatedAddress.Wardid;
        //    existingAddress.Zoneid = updatedAddress.Zoneid;
        //    existingAddress.Latitude = updatedAddress.Latitude;
        //    existingAddress.Longitude = updatedAddress.Longitude;
        //    existingAddress.Remarks = updatedAddress.Remarks;

        //    _ferrodbContext.Addresses.Update(existingAddress);
        //    await _ferrodbContext.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("DeleteAddress")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _ferrodbContext.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound("Address not found");
            }

            _ferrodbContext.Addresses.Remove(address);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
