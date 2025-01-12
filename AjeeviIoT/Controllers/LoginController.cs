using AjeeviIoT.EntitiesDTO;
using AjeeviIoT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class LoginController : ControllerBase
    {
        private readonly ferrodbContext _ferrodbContext;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ferrodbContext ferrodbContext, ILogger<LoginController> logger)
        {
            _ferrodbContext = ferrodbContext;
            _logger = logger;
        }

        [HttpGet("GetAllUserRole"), AllowAnonymous]
        public async Task<IActionResult> GetAllUserRole()
        {
            try
            {
                var userRoles = await _ferrodbContext.Userroles
                    .OrderByDescending(x => x.Id)
                    .Select(x => new UserRoleResponseDto
                    {
                        Id = x.Id,
                        RoleType = x.RoleType
                    })
                    .ToListAsync();

                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching user roles: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching user roles.");
            }
        }

        //// GET: api/Auth/GetUserRoleById/{id}
        //[HttpGet("GetUserRoleById/{id}")]
        //public async Task<IActionResult> GetUserRoleById(int id)
        //{
        //    try
        //    {
        //        var userRole = await _ferrodbContext.Userroles.FindAsync(id);

        //        if (userRole == null)
        //        {
        //            return NotFound("User role not found.");
        //        }

        //        var response = new UserRoleResponseDto
        //        {
        //            Id = userRole.Id,
        //            RoleType = userRole.RoleType
        //        };

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error fetching user role by ID: {Message}", ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the user role.");
        //    }
        //}

        // POST: api/Auth/CreateUserRole


        [HttpPost("CreateUserRole")]
        public async Task<IActionResult> CreateUserRole([FromBody] UserRoleCreateDto userRoleDto)
        {
            try
            {
                if (string.IsNullOrEmpty(userRoleDto.RoleType))
                {
                    return BadRequest("Invalid user role data.");
                }

                var userRole = new Userrole
                {
                    RoleType = userRoleDto.RoleType
                };

                _ferrodbContext.Userroles.Add(userRole);
                await _ferrodbContext.SaveChangesAsync();

                var response = new UserRoleResponseDto
                {
                    Id = userRole.Id,
                    RoleType = userRole.RoleType
                };

                // Ensure the method name matches
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating user role: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user role.");
            }
        }

        [HttpPut("UpdateUserRole/{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRoleUpdateDto userRoleDto)
        {
            try
            {
                if (id != userRoleDto.Id || string.IsNullOrEmpty(userRoleDto.RoleType))
                {
                    return BadRequest("User role data is invalid.");
                }

                var existingRole = await _ferrodbContext.Userroles.FindAsync(id);
                if (existingRole == null)
                {
                    return NotFound("User role not found.");
                }

                existingRole.RoleType = userRoleDto.RoleType;

                _ferrodbContext.Userroles.Update(existingRole);
                await _ferrodbContext.SaveChangesAsync();

                var response = new UserRoleResponseDto
                {
                    Id = existingRole.Id,
                    RoleType = existingRole.RoleType
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating user role: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the user role.");
            }
        }

        [HttpDelete("DeleteUserRole/{id}")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            try
            {
                var userRole = await _ferrodbContext.Userroles.FindAsync(id);
                if (userRole == null)
                {
                    return NotFound("User role not found.");
                }

                _ferrodbContext.Userroles.Remove(userRole);
                await _ferrodbContext.SaveChangesAsync();

                return Ok("User role deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting user role: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the user role.");
            }
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserLogin>> CreateUserLogin([FromBody] CreateUserLoginDto createUserLoginDto)
        {
            try
            {
                var userLogin = new UserLogin
                {
                    UserFname = createUserLoginDto.UserFname,
                    UserLname = createUserLoginDto.UserLname,
                    Email = createUserLoginDto.Email,
                    MobileNo = createUserLoginDto.MobileNo,
                    LoginDate = DateTime.UtcNow,
                    EntityId = createUserLoginDto.EntityId,
                    Password = createUserLoginDto.Password,
                    UserroleId = createUserLoginDto.RoleId,
                    Status = createUserLoginDto.Status

                };

                _ferrodbContext.UserLogins.Add(userLogin);
                await _ferrodbContext.SaveChangesAsync();

                var userLoginType = new Userlogintype
                {
                    UserId = userLogin.UserId.ToString(),
                    RoleId = userLogin.UserroleId
                };

                _ferrodbContext.Userlogintypes.Add(userLoginType);
                await _ferrodbContext.SaveChangesAsync();

                return Ok(userLogin);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating user login: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetAllUser"), AllowAnonymous]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var userUser = await _ferrodbContext.UserLogins
                        .OrderByDescending(x => x.UserId)
                        .Select(x => new CreateUserLoginDto
                        {
                            UserId = x.UserId,
                            UserFname = x.UserFname,
                            UserLname = x.UserLname,
                            RoleId = x.UserroleId,
                            MobileNo = x.MobileNo,
                            Email = x.Email,
                            LoginDate = x.LoginDate,
                            EntityId = x.EntityId,
                            Status = x.Status
                        }).ToListAsync();

                return Ok(userUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching user.");
            }
        }


        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUserLogin(int id)
        {
            var userLogin = await _ferrodbContext.UserLogins.FindAsync(id);

            if (userLogin == null)
            {
                return NotFound();
            }

            _ferrodbContext.UserLogins.Remove(userLogin);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("editUser")]
        public async Task<IActionResult> UpdateUserLogin(int id, [FromBody] UpdateUserLoginDto updateUserLoginDto)
        {
            var userLogin = await _ferrodbContext.UserLogins.FindAsync(id);

            if (userLogin == null)
            {
                return NotFound();
            }

            userLogin.UserFname = updateUserLoginDto.UserFname;
            userLogin.UserLname = updateUserLoginDto.UserLname;
            userLogin.Email = updateUserLoginDto.Email;
            userLogin.MobileNo = updateUserLoginDto.MobileNo;
            userLogin.EntityId = updateUserLoginDto.EntityId;
            userLogin.Password = updateUserLoginDto.Password;
            userLogin.UserroleId = updateUserLoginDto.RoleId;

            _ferrodbContext.UserLogins.Update(userLogin);
            await _ferrodbContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
