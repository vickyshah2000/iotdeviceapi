using AjeeviIoT.EntitiesDTO;
using AjeeviIoT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql;
using System.Security.Principal;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class EntityController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;
        private readonly ILogger<EntityController> _logger;

        public EntityController(ferrodbContext ferrodbContext, ILogger<EntityController> logger)
        {
            _ferrodbContext = ferrodbContext;
            _logger = logger;
        }

        [HttpGet("GetAllImageType")]
        public async Task<IActionResult> GetAllImageType()
        {
            var imgt = await _ferrodbContext.Imagetypes.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(imgt);
        }

        [HttpGet("GetAllImage")]
        public async Task<IActionResult> GetAllImageDetail()
        {
            var imgdtl = await _ferrodbContext.Imagedetails.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(imgdtl);
        }

        [HttpPost("CreateEntity")]
        public async Task<IActionResult> CreateEntity([FromForm] CreateEntityDTO createEntityDTO)
        {
            if (createEntityDTO == null)
            {
                return BadRequest("Entity data is null.");
            }

            var entity = new Entity
            {
                Name = createEntityDTO.Name,
                ShortName = createEntityDTO.ShortName,
                LongName = createEntityDTO.LongName,
                TradeName = createEntityDTO.TradeName,
                EntityTypeId = createEntityDTO.EntityTypeId,
                EntityRoleId = createEntityDTO.EntityRoleId,
                Remarks = createEntityDTO.Remarks
            };

            var strategy = _ferrodbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _ferrodbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _ferrodbContext.Entities.Add(entity);
                        await _ferrodbContext.SaveChangesAsync();

                        var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                        if (!Directory.Exists(imageFolder))
                        {
                            Directory.CreateDirectory(imageFolder);
                        }

                        var imageUrls = new List<string>(); // Store all image URLs

                        if (createEntityDTO.Images != null && createEntityDTO.Images.Any())
                        {
                            foreach (var image in createEntityDTO.Images)
                            {
                                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                                var filePath = Path.Combine(imageFolder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                var imageUrl = $"/Images/{fileName}";
                                imageUrls.Add(imageUrl);
                            }

                            var imagedetail = new Imagedetail
                            {
                                Imageurl = string.Join(",", imageUrls),
                                ImageTypeId = 3,
                                UniqueId = entity.Id.ToString()
                            };

                            _ferrodbContext.Imagedetails.Add(imagedetail);
                        }
                        else
                        {
                            var imagedetail = new Imagedetail
                            {
                                Imageurl = null,
                                ImageTypeId = 3,
                                UniqueId = entity.Id.ToString()
                            };

                            _ferrodbContext.Imagedetails.Add(imagedetail);
                        }

                        await _ferrodbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });

            return CreatedAtAction(nameof(GetEntityById), new { id = entity.Id }, entity);
        }

        [HttpPut("UpdateEntity/{id}")]
        public async Task<IActionResult> UpdateEntity(int id, [FromForm] CreateEntityDTO updateEntityDTO)
        {
            if (updateEntityDTO == null)
            {
                return BadRequest("Entity data is null.");
            }

            var existingEntity = await _ferrodbContext.Entities
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEntity == null)
            {
                return NotFound($"Entity with ID {id} not found.");
            }

            // Update entity properties
            existingEntity.Name = updateEntityDTO.Name;
            existingEntity.ShortName = updateEntityDTO.ShortName;
            existingEntity.LongName = updateEntityDTO.LongName;
            existingEntity.TradeName = updateEntityDTO.TradeName;
            existingEntity.EntityTypeId = updateEntityDTO.EntityTypeId;
            existingEntity.EntityRoleId = updateEntityDTO.EntityRoleId;
            existingEntity.Remarks = updateEntityDTO.Remarks;

            // Wrap the entire database operation in an execution strategy
            var strategy = _ferrodbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _ferrodbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Update the entity in the database
                        _ferrodbContext.Entities.Update(existingEntity);
                        await _ferrodbContext.SaveChangesAsync();

                        // Handle image updating
                        var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                        if (!Directory.Exists(imageFolder))
                        {
                            Directory.CreateDirectory(imageFolder);
                        }

                        // Handle image upload (if new images are provided)
                        if (updateEntityDTO.Images != null && updateEntityDTO.Images.Any())
                        {
                            // Delete existing images associated with the entity
                            var existingImages = await _ferrodbContext.Imagedetails
                                .Where(i => i.UniqueId == existingEntity.Id.ToString())
                                .ToListAsync();
                            _ferrodbContext.Imagedetails.RemoveRange(existingImages);
                            await _ferrodbContext.SaveChangesAsync();

                            // Add new images
                            foreach (var image in updateEntityDTO.Images)
                            {
                                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                                var filePath = Path.Combine(imageFolder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                var imagedetail = new Imagedetail
                                {
                                    Imageurl = $"/Images/{fileName}",
                                    ImageTypeId = 3,
                                    UniqueId = existingEntity.Id.ToString()
                                };

                                _ferrodbContext.Imagedetails.Add(imagedetail);
                            }
                        }
                        else
                        {
                            // If no images provided, delete the old ones (if any)
                            var existingImages = await _ferrodbContext.Imagedetails
                                .Where(i => i.UniqueId == existingEntity.Id.ToString())
                                .ToListAsync();
                            _ferrodbContext.Imagedetails.RemoveRange(existingImages);
                        }

                        await _ferrodbContext.SaveChangesAsync();

                        // Commit the transaction
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        // Rollback if an error occurs
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });

            return Ok(new { message = "Entity updated successfully", Entity = existingEntity });
        }

        [HttpGet("GetEntityById")]
        public async Task<IActionResult> GetEntityById(int entityId)
        {
            try
            {
                var entityWithImages = await (from e in _ferrodbContext.Entities.OrderByDescending(i => i.Id)
                                              join imd in
                                                  (from img in _ferrodbContext.Imagedetails
                                                   where img.ImageTypeId == 3
                                                   select img)
                                              on e.Id.ToString() equals imd.UniqueId into imdGroup
                                              from imd in imdGroup.DefaultIfEmpty()
                                              join et in _ferrodbContext.Entitytypes on e.EntityTypeId equals et.TypeId into etGroup
                                              from et in etGroup.DefaultIfEmpty()
                                              join er in _ferrodbContext.Entityroles on e.EntityRoleId equals er.RoleId into erGroup
                                              from er in erGroup.DefaultIfEmpty()
                                              where e.Id == entityId
                                              select new
                                              {
                                                  EntityId = e.Id,
                                                  Name = e.Name,
                                                  ShortName = e.ShortName,
                                                  LongName = e.LongName,
                                                  TradeName = e.TradeName,
                                                  EntityTypeId = e.EntityTypeId,
                                                  EntityRoleId = e.EntityRoleId,
                                                  EntityTypeName = et.EntityTypeName,
                                                  RoleName = er.Rolename,
                                                  Remarks = e.Remarks,
                                                  ImageUrl = imd.Imageurl
                                              })
                                              .FirstOrDefaultAsync();

                // Check if the entity exists
                if (entityWithImages == null)
                {
                    return NotFound($"Entity with ID {entityId} not found.");
                }

                return Ok(entityWithImages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching entity: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        [HttpGet("GetAllEntities")]
        public async Task<IActionResult> GetAllEntities(int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var totalCount = await _ferrodbContext.Entities.CountAsync();

                var query = from e in _ferrodbContext.Entities.OrderByDescending(e => e.Id)
                            join imd in
                                (from img in _ferrodbContext.Imagedetails
                                 where img.ImageTypeId == 3
                                 select img)
                            on e.Id.ToString() equals imd.UniqueId into imdGroup
                            from imd in imdGroup.DefaultIfEmpty()
                            join et in _ferrodbContext.Entitytypes on e.EntityTypeId equals et.TypeId into etGroup
                            from et in etGroup.DefaultIfEmpty()
                            join er in _ferrodbContext.Entityroles on e.EntityRoleId equals er.RoleId into erGroup
                            from er in erGroup.DefaultIfEmpty()
                            orderby e.Id descending
                            select new
                            {
                                EntityId = e.Id,
                                Name = e.Name,
                                ShortName = e.ShortName,
                                LongName = e.LongName,
                                TradeName = e.TradeName,
                                EntityTypeName = et.EntityTypeName,
                                RoleName = er.Rolename,
                                Remarks = e.Remarks,
                                ImageUrl = imd.Imageurl
                            };


                if (pageNumber.HasValue && pageSize.HasValue)
                {
                    query = query
                        .Skip((pageNumber.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);
                }

                var entityWithImages = await query.ToListAsync();

                return Ok(new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? totalCount,
                    Data = entityWithImages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching entities: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        [HttpDelete("DeleteEntity")]
        public async Task<IActionResult> DeleteEntity(int id)
        {
            var entity = await _ferrodbContext.Entities.FindAsync(id);

            if (entity == null)
            {
                return NotFound($"Entity with Id {id} not found.");
            }

            _ferrodbContext.Entities.Remove(entity);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("CreateAddressForEntity")]
        public async Task<IActionResult> CreateAddressForEntity([FromBody] CreateEntityAddressDTO createAddressDTO)
        {
            if (createAddressDTO == null || createAddressDTO.Addresses == null || !createAddressDTO.Addresses.Any())
            {
                return BadRequest("Invalid Entity Address data");
            }

            var createdAddresses = new List<object>();

            // Loop through each address in the Addresses list
            foreach (var addressDTO in createAddressDTO.Addresses)
            {
                var newAddress = new Address
                {
                    Addressline1 = addressDTO.Addressline1,
                    Addressline2 = addressDTO.Addressline2,
                    Pincode = addressDTO.Pincode,
                    CityId = addressDTO.CityId,
                    Stateid = addressDTO.StateId,
                    Wardid = addressDTO.WardId,
                    Regionid = addressDTO.RegionId,
                    Zoneid = addressDTO.ZoneId,
                    Latitude = addressDTO.Latitude,
                    Longitude = addressDTO.Longitude,
                    CountryId = addressDTO.CountryId,
                    Remarks = addressDTO.Remarks
                };

                _ferrodbContext.Addresses.Add(newAddress);
                await _ferrodbContext.SaveChangesAsync();  // Save the address

                // Create Entityaddress record
                var newEntityAddress = new Entityaddress
                {
                    Entityid = createAddressDTO.EntityId,  // Associate with EntityId
                    Addressid = newAddress.Id,  // Link to AddressId
                    Addresstype = addressDTO.AddressType
                };

                _ferrodbContext.Entityaddresses.Add(newEntityAddress);

                createdAddresses.Add(new
                {
                    AddressId = newAddress.Id,
                    Addressline1 = newAddress.Addressline1,
                    Addressline2 = newAddress.Addressline2,
                    Pincode = newAddress.Pincode,
                    CityId = newAddress.CityId,
                    StateId = newAddress.Stateid,
                    WardId = newAddress.Wardid,
                    RegionId = newAddress.Regionid,
                    ZoneId = newAddress.Zoneid,
                    Latitude = newAddress.Latitude,
                    Longitude = newAddress.Longitude,
                    CountryId = newAddress.CountryId,
                    Remarks = newAddress.Remarks,
                    AddressType = newEntityAddress.Addresstype
                });
            }

            // Save all Entityaddresses in bulk
            await _ferrodbContext.SaveChangesAsync();




            return Ok(new { message = "Addresses created successfully", CreatedAddresses = createdAddresses });
        }

        [HttpPut("UpdateAddressForEntity/{addressId}")]
        public async Task<IActionResult> UpdateAddressForEntity(int addressId, [FromBody] CreateEntityAddressDTO updateAddressDTO)
        {
            if (updateAddressDTO == null || updateAddressDTO.Addresses == null || !updateAddressDTO.Addresses.Any())
            {
                return BadRequest("Invalid Entity Address data");
            }

            var updatedAddresses = new List<object>();

            // Loop through each address in the Addresses list to update
            foreach (var addressDTO in updateAddressDTO.Addresses)
            {
                // Check if the address exists
                var existingAddress = await _ferrodbContext.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId);

                if (existingAddress == null)
                {
                    return NotFound($"Address with ID {addressId} not found.");
                }

                existingAddress.Addressline1 = addressDTO.Addressline1;
                existingAddress.Addressline2 = addressDTO.Addressline2;
                existingAddress.Pincode = addressDTO.Pincode;
                existingAddress.CityId = addressDTO.CityId;
                existingAddress.Stateid = addressDTO.StateId;
                existingAddress.Wardid = addressDTO.WardId;
                existingAddress.Regionid = addressDTO.RegionId;
                existingAddress.Zoneid = addressDTO.ZoneId;
                existingAddress.Latitude = addressDTO.Latitude;
                existingAddress.Longitude = addressDTO.Longitude;
                existingAddress.CountryId = addressDTO.CountryId;
                existingAddress.Remarks = addressDTO.Remarks;

                var existingEntityAddress = await _ferrodbContext.Entityaddresses
                    .FirstOrDefaultAsync(ea => ea.Addressid == addressId && ea.Entityid == updateAddressDTO.EntityId);

                if (existingEntityAddress != null)
                {
                    existingEntityAddress.Addresstype = addressDTO.AddressType;
                }

                _ferrodbContext.Addresses.Update(existingAddress);
                _ferrodbContext.Entityaddresses.Update(existingEntityAddress);

                updatedAddresses.Add(new
                {
                    AddressId = existingAddress.Id,
                    Addressline1 = existingAddress.Addressline1,
                    Addressline2 = existingAddress.Addressline2,
                    Pincode = existingAddress.Pincode,
                    CityId = existingAddress.CityId,
                    StateId = existingAddress.Stateid,
                    WardId = existingAddress.Wardid,
                    RegionId = existingAddress.Regionid,
                    ZoneId = existingAddress.Zoneid,
                    Latitude = existingAddress.Latitude,
                    Longitude = existingAddress.Longitude,
                    CountryId = existingAddress.CountryId,
                    Remarks = existingAddress.Remarks,
                    AddressType = existingEntityAddress.Addresstype
                });
            }

            // Save all changes
            await _ferrodbContext.SaveChangesAsync();

            return Ok(new { message = "Addresses updated successfully", UpdatedAddresses = updatedAddresses });
        }

        //[HttpGet("GetAddressesByEntityId/{entityId}")]
        //public async Task<IActionResult> GetAddressesByEntityId(int entityId, int pageNumber = 1, int pageSize = 100)
        //{
        //    if (entityId <= 0)
        //    {
        //        return BadRequest("Invalid EntityId.");
        //    }

        //    var totalCount = await _ferrodbContext.Entityaddresses
        //        .Where(ea => ea.Entityid == entityId)
        //        .CountAsync();

        //    if (totalCount == 0)
        //    {
        //        return NotFound($"No addresses found for EntityId {entityId}");
        //    }

        //    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        //    var entityAddresses = await _ferrodbContext.Entityaddresses
        //        .Where(ea => ea.Entityid == entityId)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.City)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.State)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.Country)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.Ward)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.Region)
        //        .Include(ea => ea.AddresstypeNavigation)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var result = entityAddresses.Select(ea => new
        //    {
        //        EntityAddressId = ea.Id,
        //        AddressId = ea.Addressid,
        //        Addressline1 = ea.Address.Addressline1,
        //        Addressline2 = ea.Address.Addressline2,
        //        Pincode = ea.Address.Pincode,
        //        CityId = ea.Address.CityId,
        //        CityName = ea.Address.City?.Cityname,
        //        StateId = ea.Address.Stateid,
        //        StateName = ea.Address.State?.Statename,
        //        CountryId = ea.Address.CountryId,
        //        CountryName = ea.Address.Country?.Name,
        //        WardId = ea.Address.Wardid,
        //        WardName = ea.Address.Ward?.Wardname,
        //        RegionId = ea.Address.Regionid,
        //        RegionName = ea.Address.Region.Regionname,
        //        ZoneId = ea.Address.Zoneid,
        //        Latitude = ea.Address.Latitude,
        //        Longitude = ea.Address.Longitude,
        //        Remarks = ea.Address.Remarks,
        //        AddressType = ea.Addresstype,
        //        Name = ea.AddresstypeNavigation.Name
        //    });

        //    return Ok(new
        //    {
        //        EntityId = entityId,
        //        TotalCount = totalCount,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalPages = totalPages,
        //        Addresses = result
        //    });
        //}


        [AllowAnonymous]
        [HttpGet("GetAllEntityAddresses")]
        public async Task<IActionResult> GetAllEntityAddresses(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var skip = (pageNumber - 1) * pageSize;

            var entityAddresses = await _ferrodbContext.Entityaddresses.OrderByDescending(x => x.Id)
                .Include(ea => ea.Address)
                    .ThenInclude(a => a.City)
                .Include(ea => ea.Address)
                    .ThenInclude(a => a.State)
                .Include(ea => ea.Address)
                    .ThenInclude(a => a.Country)
                .Include(ea => ea.Address)
                    .ThenInclude(a => a.Ward)
                .Include(ea => ea.Address)
                    .ThenInclude(a => a.Region)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            if (entityAddresses == null || !entityAddresses.Any())
            {
                return NotFound("No addresses found.");
            }

            var result = entityAddresses.Select(ea => new
            {
                EntityAddressId = ea.Id,
                AddressId = ea.Addressid,
                Addressline1 = ea.Address.Addressline1,
                Addressline2 = ea.Address.Addressline2,
                Pincode = ea.Address.Pincode,
                CityId = ea.Address.CityId,
                CityName = ea.Address.City?.Cityname,
                StateId = ea.Address.Stateid,
                StateName = ea.Address.State?.Statename,
                CountryId = ea.Address.CountryId,
                CountryName = ea.Address.Country?.Name,
                WardId = ea.Address.Wardid,
                WardName = ea.Address.Ward?.Wardname,
                RegionId = ea.Address.Regionid,
                RegionName = ea.Address.Region.Regionname,
                ZoneId = ea.Address.Zoneid,
                Latitude = ea.Address.Latitude,
                Longitude = ea.Address.Longitude,
                Remarks = ea.Address.Remarks,
                AddressType = ea.Addresstype
            });

            var totalCount = await _ferrodbContext.Entityaddresses.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Addresses = result
            });
        }

        [HttpPost("CreatePerson")]
        public async Task<IActionResult> CreatePerson([FromForm] CreatePersonDTO createPersonDTO)
        {
            if (createPersonDTO == null)
            {
                return BadRequest("Person data is null.");
            }

            var entityAddress = await _ferrodbContext.Addresses
                .FirstOrDefaultAsync(ea => ea.Id == createPersonDTO.AddressId);

            if (entityAddress == null)
            {
                return NotFound("EntityAddress not found.");
            }

            var person = new Person
            {
                Firstname = createPersonDTO.Firstname,
                Lastname = createPersonDTO.Lastname,
                Mobilenumber = createPersonDTO.Mobilenumber,
                Email = createPersonDTO.Email,
                Contacttype = createPersonDTO.Contacttype,
                Alternatenumber = createPersonDTO.Alternatenumber,
                Remarks = createPersonDTO.Remarks
            };

            _ferrodbContext.People.Add(person);
            await _ferrodbContext.SaveChangesAsync();

            var addressContact = new Addresscontact
            {
                Addressid = createPersonDTO.AddressId,
                Personid = person.Id
            };

            _ferrodbContext.Addresscontacts.Add(addressContact);
            await _ferrodbContext.SaveChangesAsync();

            var imageUrls = new List<string>();  // Store all image URLs

            if (createPersonDTO.Images != null && createPersonDTO.Images.Any())
            {
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                foreach (var image in createPersonDTO.Images)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                    var filePath = Path.Combine(imageFolder, fileName);

                    // Save the image to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Add the image URL to the list
                    var imageUrl = $"/Images/{fileName}";
                    imageUrls.Add(imageUrl);
                }

                // Save all URLs in a single entry
                var imagedetail = new Imagedetail
                {
                    Imageurl = string.Join(",", imageUrls),
                    ImageTypeId = 5,
                    UniqueId = person.Id.ToString()
                };

                _ferrodbContext.Imagedetails.Add(imagedetail);

                await _ferrodbContext.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Person created successfully",
                CreatedPerson = new
                {
                    person.Id,
                    person.Firstname,
                    person.Lastname,
                    person.Mobilenumber,
                    person.Email,
                    person.Contacttype,
                    person.Alternatenumber,
                    person.Remarks,
                    Images = imageUrls
                },
                AddressContact = new
                {
                    AddresscontactId = addressContact.Id,
                    addressContact.Addressid,
                    addressContact.Personid
                }
            });
        }

        [HttpGet("GetPersonById")]
        public async Task<IActionResult> GetPersonById(int personId)
        {
            try
            {
                if (personId <= 0)
                {
                    return BadRequest("Invalid Person ID.");
                }

                var personDetails = await (from ac in _ferrodbContext.Addresscontacts.OrderByDescending(a => a.Personid)
                                           join p in _ferrodbContext.People on ac.Personid equals p.Id
                                           join imd in
                                               (from img in _ferrodbContext.Imagedetails
                                                where img.ImageTypeId == 5
                                                select img)
                                           on p.Id.ToString() equals imd.UniqueId into imdGroup
                                           from imd in imdGroup.DefaultIfEmpty()
                                           where p.Id == personId // Filter by PersonId
                                           select new
                                           {
                                               AddressContactId = ac.Id,
                                               AddressId = ac.Addressid,
                                               PersonId = p.Id, // PersonId fetched
                                               FirstName = p.Firstname,
                                               LastName = p.Lastname,
                                               MobileNumber = p.Mobilenumber,
                                               Email = p.Email,
                                               AlternateNumber = p.Alternatenumber,
                                               ContactType = p.Contacttype,
                                               ImageUrl = imd.Imageurl,
                                               Remarks = p.Remarks
                                           })
                                           .FirstOrDefaultAsync();

                if (personDetails == null)
                {
                    return NotFound($"No person found with ID {personId}.");
                }

                return Ok(personDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching person by ID {personId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        [HttpGet("GetAllPersons")]
        public async Task<IActionResult> GetAllPersons(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest("Page number and page size must be greater than 0.");
                }

                var totalRecords = await (from ac in _ferrodbContext.Addresscontacts
                                          join p in _ferrodbContext.People on ac.Personid equals p.Id
                                          select ac).CountAsync();

                // Calculate total pages
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var personsWithDetails = await (from ac in _ferrodbContext.Addresscontacts.OrderByDescending(ac => ac.Personid)
                                                join p in _ferrodbContext.People on ac.Personid equals p.Id
                                                join imd in
                                                    (from img in _ferrodbContext.Imagedetails
                                                     where img.ImageTypeId == 5
                                                     select img)
                                                on p.Id.ToString() equals imd.UniqueId into imdGroup
                                                from imd in imdGroup.DefaultIfEmpty()
                                                orderby p.Id descending
                                                select new
                                                {
                                                    AddressContactId = ac.Id,
                                                    AddressId = ac.Addressid,
                                                    PersonId = p.Id,
                                                    FirstName = p.Firstname,
                                                    LastName = p.Lastname,
                                                    MobileNumber = p.Mobilenumber,
                                                    Email = p.Email,
                                                    AlternateNumber = p.Alternatenumber,
                                                    ContactType = p.Contacttype,
                                                    ImageUrl = imd.Imageurl,
                                                    Remarks = p.Remarks
                                                    //Image = imd.Imageurl
                                                })
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();

                // Check if no data found
                if (personsWithDetails == null || !personsWithDetails.Any())
                {
                    return NotFound("No persons found.");
                }

                // Return paginated data
                return Ok(new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    Persons = personsWithDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching persons: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        //[HttpGet("GetPersonsByAddressId")]
        //public async Task<IActionResult> GetPersonsByAddressId(int addressId)
        //{
        //    try
        //    {
        //        if (addressId <= 0)
        //        {
        //            return BadRequest("AddressId must be greater than 0.");
        //        }

        //        var personsWithDetails = await (from ac in _ferrodbContext.Addresscontacts.OrderByDescending(ac => ac.Id)
        //                                        where ac.Addressid == addressId
        //                                        join p in _ferrodbContext.People on ac.Personid equals p.Id
        //                                        join imd in
        //                                            (from img in _ferrodbContext.Imagedetails
        //                                             where img.ImageTypeId == 5
        //                                             select img)
        //                                        on p.Id.ToString() equals imd.UniqueId into imdGroup
        //                                        from imd in imdGroup.DefaultIfEmpty()
        //                                        orderby p.Id descending
        //                                        select new
        //                                        {
        //                                            AddressContactId = ac.Id,
        //                                            AddressId = ac.Addressid,
        //                                            PersonId = p.Id,
        //                                            FirstName = p.Firstname,
        //                                            LastName = p.Lastname,
        //                                            MobileNumber = p.Mobilenumber,
        //                                            Email = p.Email,
        //                                            AlternateNumber = p.Alternatenumber,
        //                                            ContactType = p.Contacttype,
        //                                            ImageUrl = imd.Imageurl,
        //                                            Remarks = p.Remarks
        //                                            //Image = imd.Imageurl
        //                                        })
        //                                        .ToListAsync();

        //        // Check if no persons found
        //        if (personsWithDetails == null || !personsWithDetails.Any())
        //        {
        //            return NotFound($"No persons found for AddressId: {addressId}.");
        //        }

        //        // Return all persons
        //        return Ok(personsWithDetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error fetching persons for AddressId {addressId}: {ex.Message}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        //    }
        //}

        [HttpGet("GetPersonsByAddressId")]
        public async Task<IActionResult> GetPersonsByAddressId(int addressId)
        {
            try
            {
                if (addressId <= 0)
                {
                    return BadRequest("AddressId must be greater than 0.");
                }

                var personsWithDetails = await (from ac in _ferrodbContext.Addresscontacts
                                                where ac.Addressid == addressId
                                                join p in _ferrodbContext.People on ac.Personid equals p.Id
                                                join imd in
                                                    (from img in _ferrodbContext.Imagedetails
                                                     where img.ImageTypeId == 5
                                                     select img)
                                                on p.Id.ToString() equals imd.UniqueId into imdGroup
                                                from imd in imdGroup.DefaultIfEmpty()
                                                join ct in _ferrodbContext.Contacttypes
                                                on p.Contacttype equals ct.ContactTypeId into ctGroup
                                                from ct in ctGroup.DefaultIfEmpty()
                                                orderby ac.Id descending
                                                select new
                                                {
                                                    AddressContactId = ac.Id,
                                                    AddressId = ac.Addressid,
                                                    PersonId = p.Id,
                                                    FirstName = p.Firstname,
                                                    LastName = p.Lastname,
                                                    MobileNumber = p.Mobilenumber,
                                                    Email = p.Email,
                                                    AlternateNumber = p.Alternatenumber,
                                                    ContactTypeId = ct != null ? ct.ContactTypeId : (int?)null,
                                                    ContactTypeName = ct != null ? ct.Contacttypename : null,
                                                    ImageUrl = imd != null ? imd.Imageurl : null,
                                                    Remarks = p.Remarks
                                                })
                                                .ToListAsync();

                if (personsWithDetails == null || !personsWithDetails.Any())
                {
                    return NotFound($"No persons found for AddressId: {addressId}.");
                }

                return Ok(personsWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching persons for AddressId {addressId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        [HttpPut("UpdatePersonById")]
        public async Task<IActionResult> UpdatePerson(int personId, [FromForm] UpdatePersonDTO updatePersonDTO)
        {
            if (updatePersonDTO == null)
            {
                return BadRequest("Person data is null.");
            }

            try
            {
                // Fetch the existing person
                var person = await _ferrodbContext.People.FindAsync(personId);
                if (person == null)
                {
                    return NotFound($"Person with ID {personId} not found.");
                }

                // Update person details
                person.Firstname = updatePersonDTO.Firstname;
                person.Lastname = updatePersonDTO.Lastname;
                person.Mobilenumber = updatePersonDTO.Mobilenumber;
                person.Email = updatePersonDTO.Email;
                person.Contacttype = updatePersonDTO.Contacttype;
                person.Alternatenumber = updatePersonDTO.Alternatenumber;
                person.Remarks = updatePersonDTO.Remarks;

                _ferrodbContext.People.Update(person);
                await _ferrodbContext.SaveChangesAsync();

                // Update AddressContact if EntityAddressId is provided
                if (updatePersonDTO.AddressId.HasValue)
                {
                    var addressContact = await _ferrodbContext.Addresscontacts
                        .FirstOrDefaultAsync(ac => ac.Personid == personId);

                    if (addressContact != null)
                    {
                        addressContact.Addressid = updatePersonDTO.AddressId.Value;
                        _ferrodbContext.Addresscontacts.Update(addressContact);
                        await _ferrodbContext.SaveChangesAsync();
                    }
                }

                // Handle image updates
                if (updatePersonDTO.Images != null && updatePersonDTO.Images.Any())
                {
                    // Fetch existing images
                    var existingImages = await _ferrodbContext.Imagedetails
                        .Where(img => img.UniqueId == personId.ToString() && img.ImageTypeId == 5)
                        .ToListAsync();

                    // Delete existing images from disk and database
                    var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                    foreach (var existingImage in existingImages)
                    {
                        var existingFilePath = Path.Combine(imageFolder, Path.GetFileName(existingImage.Imageurl));
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }

                        _ferrodbContext.Imagedetails.Remove(existingImage);
                    }

                    await _ferrodbContext.SaveChangesAsync();

                    // Add new images
                    foreach (var image in updatePersonDTO.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        var filePath = Path.Combine(imageFolder, fileName);

                        // Save the new image to disk
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Add the new image details to the database
                        var newImage = new Imagedetail
                        {
                            Imageurl = $"/Images/{fileName}",
                            ImageTypeId = 5,  // Assuming 5 is for person images
                            UniqueId = personId.ToString()
                        };

                        _ferrodbContext.Imagedetails.Add(newImage);
                    }

                    await _ferrodbContext.SaveChangesAsync();
                }

                return Ok(new
                {
                    message = "Person updated successfully",
                    UpdatedPerson = new
                    {
                        person.Id,
                        person.Firstname,
                        person.Lastname,
                        person.Mobilenumber,
                        person.Email,
                        person.Contacttype,
                        person.Alternatenumber,
                        person.Remarks
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating person with ID {personId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        [HttpGet("GetEntityRelationshipByEntityId")]
        public async Task<IActionResult> GetEntityRelationshipByEntityId(int entityId)
        {
            try
            {
                var result = await (
                    from er in _ferrodbContext.Entityrelationships.OrderByDescending(r => r.Id)
                    join e in _ferrodbContext.Entities on er.Entityid equals e.Id into entityJoin
                    from e in entityJoin.DefaultIfEmpty()
                    join subQuery in (
                        from ee in _ferrodbContext.Entities
                        join err in _ferrodbContext.Entityroles on ee.EntityRoleId equals err.RoleId into roleJoin
                        from err in roleJoin.DefaultIfEmpty()
                        join et in _ferrodbContext.Entitytypes on ee.EntityTypeId equals et.TypeId into typeJoin
                        from et in typeJoin.DefaultIfEmpty()
                        select new
                        {
                            Id = ee.Id,
                            Name = ee.Name,
                            ShortName = ee.ShortName,
                            LongName = ee.LongName,
                            TradeName = ee.TradeName,
                            RoleName = err != null ? err.Rolename : null,
                            EntityTypeName = et != null ? et.EntityTypeName : null,
                            Remarks = ee.Remarks
                        }
                    ) on er.RelationshipId equals subQuery.Id into relationshipJoin
                    from subQuery in relationshipJoin.DefaultIfEmpty()
                    join erl in _ferrodbContext.Entityroles on er.EntityroleId equals erl.RoleId into entityRoleJoin
                    from erl in entityRoleJoin.DefaultIfEmpty()
                    where er.Entityid == entityId
                    select new
                    {
                        EntityId = er.Entityid,
                        RelationshipId = er.RelationshipId,
                        Name = subQuery.Name ?? null,
                        ShortName = subQuery.ShortName ?? null,
                        LongName = subQuery.LongName ?? null,
                        TradeName = subQuery.TradeName ?? null,
                        RoleName = subQuery.RoleName ?? null,
                        EntityTypeName = subQuery.EntityTypeName ?? null,
                        Remarks = subQuery.Remarks ?? null,
                        EntityRoleName = erl.Rolename ?? null
                    }
                ).ToListAsync();

                if (!result.Any())
                {
                    return NotFound(new { message = "No data found for the given entityId." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //[HttpGet("GetAddressesByEntityId")]
        //public async Task<IActionResult> GetAddressesByEntityId(int entityId, int? pageNumber = null, int? pageSize = null)
        //{
        //    if (entityId <= 0)
        //    {
        //        return BadRequest("Invalid EntityId.");
        //    }

        //    var query = _ferrodbContext.Entityaddresses.OrderByDescending(x => x.Id)
        //        .Where(ea => ea.Entityid == entityId)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.City)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.State)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.Country)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.Ward)
        //        .Include(ea => ea.Address)
        //            .ThenInclude(a => a.Region)
        //         .Include(ea => ea.AddresstypeNavigation)
        //        .AsQueryable();

        //    var totalCount = await query.CountAsync();

        //    if (totalCount == 0)
        //    {
        //        return NotFound($"No addresses found for EntityId {entityId}");
        //    }

        //    // Apply pagination only if pageNumber and pageSize are provided 
        //    if (pageNumber.HasValue && pageSize.HasValue)
        //    {
        //        query = query
        //            .Skip((pageNumber.Value - 1) * pageSize.Value)
        //            .Take(pageSize.Value);
        //    }

        //    var entityAddresses = await query.ToListAsync();

        //    var result = entityAddresses.Select(ea => new
        //    {
        //        EntityAddressId = ea.Id,
        //        AddressId = ea.Addressid,
        //        Addressline1 = ea.Address.Addressline1,
        //        Addressline2 = ea.Address.Addressline2,
        //        Pincode = ea.Address.Pincode,
        //        CityId = ea.Address.CityId,
        //        CityName = ea.Address.City?.Cityname,
        //        StateId = ea.Address.Stateid,
        //        StateName = ea.Address.State?.Statename,
        //        CountryId = ea.Address.CountryId,
        //        CountryName = ea.Address.Country?.Name,
        //        WardId = ea.Address.Wardid,
        //        WardName = ea.Address.Ward?.Wardname,
        //        RegionId = ea.Address.Regionid,
        //        RegionName = ea.Address.Region.Regionname,
        //        ZoneId = ea.Address.Zoneid,
        //        Latitude = ea.Address.Latitude,
        //        Longitude = ea.Address.Longitude,
        //        Remarks = ea.Address.Remarks,
        //        AddressType = ea.Addresstype,
        //        Name = ea.AddresstypeNavigation.Name
        //    });

        //    return Ok(new
        //    {
        //        EntityId = entityId,
        //        TotalCount = totalCount,
        //        PageNumber = pageNumber ?? 0,
        //        PageSize = pageSize ?? 0,
        //        TotalPages = pageSize.HasValue && pageSize > 0
        //            ? (int)Math.Ceiling(totalCount / (double)pageSize.Value)
        //            : 1,
        //        Addresses = result
        //    });
        //}


        [AllowAnonymous]
        [HttpGet("GetAddressesByEntityId")]
        public ActionResult GetAddressesByEntityId(int entityId, int? pageNumber = 1, int? pageSize = 10)
        {
            if (entityId <= 0)
            {
                return BadRequest("Invalid EntityId.");
            }

            try
            {
                int defaultPageNumber = 1;
                int defaultPageSize = 10;
                pageNumber ??= defaultPageNumber;
                pageSize ??= defaultPageSize;



                var sqlQuerycount = $"select Count(Id) as TotalCount from entityaddress where  entityid={entityId}";

                var rDataCount = DBContextData.ExecuteQueryTotalCount(sqlQuerycount);

                int totalCount = rDataCount;


                // Raw SQL Query
                var sqlQuery = @"
                                select er.entityid,aa.*,atp.id addressTypeId,atp.name addressType from entityaddress ed
                                left join entityrelationship er on ed.entityid=er.RelationshipId
                                left join(
                                SELECT 
                                a.id, a.addressline1, a.Addressline2, a.pincode,c.City_Id, c.cityname,s.State_Id, s.statename,w.Ward_Id, w.wardname,r.Region_Id,
                                r.regionname,z.Zone_Id, z.zonename, a.longitude, a.latitude, cc.name AS CountryName, a.remarks 
                                FROM address a
                                LEFT JOIN city c ON a.CityId = c.City_Id
                                LEFT JOIN state s ON a.stateid = s.State_Id
                                LEFT JOIN ward w ON a.wardid = w.Ward_Id
                                LEFT JOIN region r ON a.regionid = r.Region_Id
                                LEFT JOIN zone z ON a.zoneid = z.Zone_Id
                                LEFT JOIN country cc ON a.CountryId = cc.name) aa on ed.addressid=aa.id
                                left join address_type atp on atp.id=ed.addresstype
                                where";

                sqlQuery = sqlQuery + $" er.entityid={entityId} limit {pageSize}  offset {(pageNumber-1)*pageSize};";
           

               // sqlQuery = sqlQuery.Replace("@EntityId", entityId.ToString());
               

                var rData = DBContextData.ExecuteQueryDynamicDataset(sqlQuery);

                if(rData!= null)
                {
                    //rData = rData.Replace("\r\n", "");
                    var rcdaat = JsonObject.Parse(rData);
                    return Ok(new
                    {
                        EntityId = entityId,
                        TotalCount = totalCount,
                        PageNumber = pageNumber ?? 0,
                        PageSize = pageSize ?? 0,
                        TotalPages = Convert.ToInt32(totalCount / pageSize),
                        Addresses = rcdaat
                    });
                }
                else
                {
                    return BadRequest("No Data Found");
                }
            }
            catch (Exception ex)
            {
                // Log exception (not shown here for brevity)
                return BadRequest(ex.ToString());
            }
        }

    }
}
